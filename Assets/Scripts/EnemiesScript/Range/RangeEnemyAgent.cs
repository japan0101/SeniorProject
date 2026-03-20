using EnemiesScript.Boss;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;


namespace EnemiesScript.Range
{
    public class RangeEnemyAgent : EnemyAgent
    {
        private bool attackedThisStep;
        private float lastDistance;
        private float minAttackDistance = 1.5f;

        //float Timer = 0;//for testing agent action remove later

        private new void Awake()
        {
            base.Awake();
            agent._player = GameObject.FindGameObjectWithTag("Enemy");
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


            if (Input.GetKey(KeyCode.LeftShift)) discreteActionsOut[0] = 1; //Dash
            if (Input.GetKey(KeyCode.Space)) discreteActionsOut[1] = 1; //attack
            if (Input.GetKey(KeyCode.Q)) continuousActions[2] = -1; //ccwspin
            if (Input.GetKey(KeyCode.E)) continuousActions[2] = 1; //cwspin
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            if (agent._player != null)
            {
                var toPlayer = agent._player.transform.position - transform.position;
                sensor.AddObservation(toPlayer.normalized);
                sensor.AddObservation(toPlayer.magnitude / 30f);
            }
            else
            {
                sensor.AddObservation(Vector3.zero);
                sensor.AddObservation(0f);
            }

            sensor.AddObservation(transform.forward);
            sensor.AddObservation(agent.energy);
            sensor.AddObservation(transform.localPosition); // FIX: use localPosition

            // Add boss health so range enemy knows when boss is weak
            if (agent._player != null)
            {
                var bossAgent = agent._player.GetComponent<BossEnemyAgent>();
                if (bossAgent != null)
                    sensor.AddObservation(bossAgent.agent.hp / bossAgent.agent.maxHp);
                else
                    sensor.AddObservation(1f);
            }
            else
            {
                sensor.AddObservation(1f);
            }
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

            attackedThisStep = false;

            if (attack > 0)
            {
                agent.Attack(attack - 1);
                attackedThisStep = true;
            }

            base.OnActionReceived(actions);

            if (isTraining && agent._player != null)
            {
                var player = agent._player;
                var currentDistance = Vector3.Distance(transform.position, player.transform.position);

                // Penalty for being outside optimal shooting range
                var optimalRange = 6f;
                var distanceFromOptimal = Mathf.Abs(currentDistance - optimalRange);
                AddReward(-distanceFromOptimal * 0.003f);

                // Penalize getting too close to boss (danger zone)
                if (currentDistance < 3f)
                    AddReward(-0.02f);

                // Facing reward: +0.05 when fully facing player, -0.05 when facing away
                // This is the ONLY rotation signal — no per-step rotation penalty
                // (stacked penalties caused reward collapse)
                var toPlayer = (player.transform.position - transform.position).normalized;
                var facingDot = Vector3.Dot(transform.forward, toPlayer);
                AddReward(facingDot * 0.05f);

                // Step penalty
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
            cumulativeReward = GetCumulativeReward();
        }

        public override void
            OnAttackMissed() //Called by Enemy attack event listener to notify that the attack launched did not and on a player
        {
            if (!isTraining) return;
        }

        public override void OnAttackLanded() // Called when Agent Hit Something
        {
            if (!isTraining) return;
            AddReward(1f);
            cumulativeReward = GetCumulativeReward();
        }

        public override void OnKilledTarget() // Called when Agent Kill Something
        {
            if (!isTraining) return;
            AddReward(5f);
            cumulativeReward = GetCumulativeReward();
        }

        public override void OnKilled()
        {
            if (!isTraining) return;
            AddReward(-1f);
            cumulativeReward = GetCumulativeReward();
            arenaController?.PlayerDefeated(this); // FIX: was PlayerDefeated
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

            // SpawnPlayer();
        }

        private void SpawnPlayer()
        {
            var localOrigin = new Vector3(0f, 0.5f, 0f);


            // Randomize the direction on the Y-axis (angle in degrees)
            var randomAngle = Random.Range(0f, 360f);
            var randomDirection = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;

            // Randomize the distance within range [1, 2.5]
            var randomDistance = Random.Range(1f, 10f);

            // Calculate the player's position
            var localPlayerPosition = localOrigin + randomDirection * randomDistance;

            // Apply the calculated position to the player


            // var navigation = agent._player.GetComponent<TrainerNavigation>();
            // navigation.EnemyTarget = gameObject;
            // agent._player.transform.localPosition = localPlayerPosition;
            // agent._player.transform.localRotation = Quaternion.identity;
            // agent.AddListenerToTarget(agent._player);
        }
    }
}