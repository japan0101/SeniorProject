using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;


namespace EnemiesScript.Boss
{
    public class BossEnemyAgent:EnemyAgent
    {

        //float Timer = 0;//for testing agent action remove later
        const int basicslashIndex = 1;
        const int thrustIndex = 2;
        const int warcryIndex = 3;
        const int bodyslamIndex = 4;
        const int jumpslamIndex = 5;
        const int evadeslashIndex = 6;
        private new void Awake()
        {
            base.Awake();
            agent._player = GameObject.FindGameObjectWithTag("Player");
        }

        public override void Initialize()
        {
            base.Initialize();
            if (isTraining)
            {
                currentEpisode = 0;
                cumulativeReward = 0f;
            }
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActions = actionsOut.ContinuousActions;
            var discreteActionsOut = actionsOut.DiscreteActions;
            continuousActions[0] = Input.GetAxis("Horizontal");
            continuousActions[1] = Input.GetAxis("Vertical");
            

            discreteActionsOut[0] = 0; // Do nothing
            discreteActionsOut[1] = 0; // Do nothing


            if (Input.GetKey(KeyCode.LeftShift))
            {
                discreteActionsOut[0] = 1;//Dash
            }

            if (Input.GetKey(KeyCode.Keypad1))
            {
                discreteActionsOut[1] = basicslashIndex;
            }
            if (Input.GetKey(KeyCode.Keypad2))
            {
                discreteActionsOut[1] = thrustIndex;
            }
            if (Input.GetKey(KeyCode.Keypad3))
            {
                discreteActionsOut[1] = warcryIndex;
            }
            if (Input.GetKey(KeyCode.Keypad4))
            {
                discreteActionsOut[1] = bodyslamIndex;
            }
            if (Input.GetKey(KeyCode.Keypad5))
            {
                discreteActionsOut[1] = jumpslamIndex;
            }
            if (Input.GetKey(KeyCode.Keypad6))
            {
                discreteActionsOut[1] = evadeslashIndex;
            }

            if (Input.GetKey(KeyCode.Space))
            {
                discreteActionsOut[1] = basicslashIndex;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                continuousActions[2] = -1;
            }
            if (Input.GetKey(KeyCode.E))
            {
                continuousActions[2] = 1;
            }
            
        }
        public override void CollectObservations(VectorSensor sensor)
        {
            if (agent._player != null)
            {
                Vector3 toPlayer = (agent._player.transform.position - transform.position);
                sensor.AddObservation(toPlayer.normalized);
                sensor.AddObservation(toPlayer.magnitude/30f);
            }
            else
            {
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(0f);
            }
            
            sensor.AddObservation(transform.forward);
            sensor.AddObservation(agent.energy);
            sensor.AddObservation(transform.GetChild(3).position);

            // Add angular velocity so agent knows it's spinning
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
                sensor.AddObservation(rb.angularVelocity.y / 10f);
            else
                sensor.AddObservation(0f);
        }
        public override void OnActionReceived(ActionBuffers actions)
        {
            var moveX = actions.ContinuousActions[0];
            var moveZ = actions.ContinuousActions[1];
            var rotation = actions.ContinuousActions[2];
            var special = actions.DiscreteActions[0];
            var attack = actions.DiscreteActions[1];

            agent.MoveAgentX(moveX);
            agent.MoveAgentZ(moveZ);
            agent.RotateAgent(rotation);
            agent.Specials(special);
            
            _attackedThisStep = false;

            if (attack > 0)
            {
                agent.Attack(attack);
                _attackedThisStep = true;
            }
            base.OnActionReceived(actions);
            
            if (isTraining && agent._player != null)
            {
                var player = agent._player;
                float currentDistance = Vector3.Distance(transform.position, player.transform.position);

                // Reward for being in attack range instead of just getting closer
                float optimalRange = 2.5f; // Adjust to your attack range
                float distanceFromOptimal = Mathf.Abs(currentDistance - optimalRange);
                AddReward(-distanceFromOptimal * 0.005f); // Penalize being outside optimal range

                // Penalize strafing (sideways movement) when far from player
                if (currentDistance > optimalRange + 1f)
                {
                    // moveX is sideways movement - penalize it when not in range
                    AddReward(-Mathf.Abs(moveX) * 0.01f);
                }

                // Penalize excessive rotation
                if (Mathf.Abs(rotation) > 0.1f)
                {
                    AddReward(-Mathf.Abs(rotation) * 0.005f);
                }

                // Reward for facing the player
                if (sightDetector != null && sightDetector.IsTargetVisible)
                {
                    Vector3 toPlayer = (agent._player.transform.position - transform.position).normalized;
                    float dotProduct = Vector3.Dot(transform.forward, toPlayer);

                    if (dotProduct > 0.9f)
                    {
                        AddReward(dotProduct * 0.01f);
                    }
                }

                // Small time penalty to encourage efficiency
                AddReward(-0.0001f);
                _lastDistance = currentDistance;
                cumulativeReward = GetCumulativeReward();
            }
        }

        public override void OnAttack()
        {
            if (!isTraining) return;
            // AddReward(-0.02f);
            cumulativeReward = GetCumulativeReward();
        }

        public override void OnSpecial()
        {
            if (!isTraining) return;
            AddReward(-0.01f);
            cumulativeReward = GetCumulativeReward();
        }
        public override void OnAttackMissed()//Called by Enemy attack event listener to notify that the attack launched did not and on a player
        {
            if (!isTraining) return;
        }
        public override void OnAttackLanded()// Called when Agent Hit Something
        {
            if (!isTraining) return;
            AddReward(0.05f);
            cumulativeReward = GetCumulativeReward();
        }
        public override void OnKilledTarget()// Called when Agent Kill Something
        {
            if (!isTraining) return;
            AddReward(5f);
            cumulativeReward = GetCumulativeReward();
        }
        public override void OnKilled()
        {
            // Getting Killed
            if (!isTraining) return;
            AddReward(-1f);
            cumulativeReward = GetCumulativeReward();
            arenaController?.EnemyDefeated(this);
        }

        public override void OnHurt()
        {
            if (!isTraining) return;
            AddReward(-0.01f);
            cumulativeReward = GetCumulativeReward();
        }
        
        
        public override void OnEpisodeBegin()
        {
            if (!isTraining) return;
            
            currentEpisode++;
            cumulativeReward = 0f;
            
            // Using the TrainingController the Handle the Spawn Logic
            // SpawnPlayer();
            // if (agent._player != null)
            //     _lastDistance = Vector3.Distance(transform.position, agent._player.transform.position);
        }
        
        private void SpawnPlayer()
        {
            var localOrigin = new Vector3(0f, 0.5f, 0f);
            
            
            // Randomize the direction on the Y-axis (angle in degrees)
            float randomAngle = Random.Range(0f, 360f);
            Vector3 randomDirection = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;
        
            // Randomize the distance within range [1, 2.5]
            float randomDistance = Random.Range(1f, 10f);
        
            // Calculate the player's position
            Vector3 localPlayerPosition = localOrigin + randomDirection * randomDistance;
        
            // Apply the calculated position to the player
            
            
            if (agent._player) Destroy(agent._player);
            
            agent._player.transform.localPosition = localPlayerPosition;
            agent._player.transform.localRotation = Quaternion.identity;
            agent.AddListenerToTarget(agent._player);
        }
    }
}