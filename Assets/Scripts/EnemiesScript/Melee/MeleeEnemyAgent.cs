using System.Collections;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;


namespace EnemiesScript.Melee
{
    public class MeleeEnemyAgent:EnemyAgent
    {
        
        float Timer = 0;//for testing agent action remove later

        private new void Awake()
        {
            base.Awake();
            agent._player = GameObject.FindGameObjectWithTag("Player");
        }

        public override void Initialize()
        {
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
            if (Input.GetKey(KeyCode.Space))
            {
                discreteActionsOut[1] = 1;
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
            // Give Agent the information about the state
            // Using Ray Perception to identify the goal
            sensor.AddObservation(agent.energy);
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
                agent.Attack(attack - 1);
                _attackedThisStep = true;
            }
            base.OnActionReceived(actions);
            
            if (isTraining & agent._player != null)
            {
                var player = agent._player;
                float currentDistance = Vector3.Distance(transform.position, player.transform.position);
                
                // Reward moving closer, penalize running away
                float distanceDelta = _lastDistance - currentDistance;
                AddReward(Mathf.Clamp(distanceDelta * 0.05f, -0.05f, 0.05f));

                // Reward staying in good combat range (not too far, not too close)
                float idealRange = 2.5f;
                float rangeScore = 1f - Mathf.Abs(currentDistance - idealRange) / idealRange;
                AddReward(rangeScore * 0.001f);


                // Penalize camping too close without attacking
                if (currentDistance <= _minAttackDistance && !_attackedThisStep)
                    AddReward(-0.002f);

                _lastDistance = currentDistance;
                
                // Penalty given each step to encourage agent to finish a task quickly
                AddReward(-0.0001f);
                // Survival incentive
                // AddReward(0.0001f);
                // // Update the cumulative reward after adding the step penalty.
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