using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace EnemiesScript.Melee
{
    public class MeleeEnemyAgent:Agent
    {
        [SerializeField] private GameObject target;
        [SerializeField] private Renderer groundRenderer;
        [SerializeField] private Enemy agent;
        [HideInInspector] public int currentEpisode;
        [HideInInspector] public float cumulativeReward;

        private Color _defaultGroundColor;
        private Coroutine _flashGroundCoroutine;

        float Timer = 0;//for testing agent action remove later
        public override void Initialize()
        {
            currentEpisode = 0;
            cumulativeReward = 0f;

            if (groundRenderer)
            {
                _defaultGroundColor = groundRenderer.material.color;
            }
        }

        //public override void Heuristic(in ActionBuffers actionsOut)
        //{
        //    var continuousActionsOut = actionsOut.ContinuousActions;
        //    continuousActionsOut[0] = Input.GetAxis("Horizontal");
        //    continuousActionsOut[1] = Input.GetAxis("Vertical");
        //}
        
         public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActions = actionsOut.ContinuousActions;
            var discreteActionsOut = actionsOut.DiscreteActions;
            continuousActions[0] = Input.GetAxis("Horizontal");
            continuousActions[1] = Input.GetAxis("Vertical");
            continuousActions[2] = Input.GetAxis("Mouse X");

            discreteActionsOut[0] = 0; // Do nothing
            discreteActionsOut[1] = 0; // Do nothing


            if (Input.GetKey(KeyCode.LeftShift))
            {
                discreteActionsOut[0] = 1;
            }
            
            // Shitty Control Who TF Write this
            if (Input.GetMouseButton(0))
            {
                discreteActionsOut[1] = 1;
            }
        }
        public override void CollectObservations(VectorSensor sensor)
        {
            // Give Agent the information about the state
            // The Player's position
            // Vector3 playerPosNormalized = _player.localPosition.normalized;

            // The Enemy's position


            // The Enemy's direction (on the Y Axis)


            // sensor.AddObservation(playerPosNormalized.x);
            // sensor.AddObservation(playerPosNormalized.z); 

            // Using Ray Perception to identify the goal
            sensor.AddObservation(transform.localPosition);
        }
        public override void OnActionReceived(ActionBuffers actions)
        {
            // var xVelocity = actions.ContinuousActions[0];
            //var zVelocity = actions.ContinuousActions[1];
            
            //var xDistance = xVelocity * maxSpeed * Time.fixedDeltaTime;
            //var zDistance = zVelocity * maxSpeed * Time.fixedDeltaTime;
            
            //transform.localPosition += new Vector3(xDistance, 0, zDistance);
            
            // Penalty given each step to encourage agent to finish a task quickly
            var moveX = actions.ContinuousActions[0];
            var moveZ = actions.ContinuousActions[1];
            // var movement = actions.DiscreteActions[0];
            var rotation = actions.ContinuousActions[2];
            var special = actions.DiscreteActions[0];
            var attack = actions.DiscreteActions[1];

            agent.MoveAgentX(moveX);
            agent.MoveAgentZ(moveZ);
            agent.RotateAgent(rotation);
            agent.Specials(special);

            if (attack > 0)
            {
                agent.Attack(attack - 1);
            }
            base.OnActionReceived(actions);

            AddReward(-2f/MaxStep);
            // // Update the cumulative reward after adding the step penalty.
            cumulativeReward = GetCumulativeReward();
        }

        public void OnKilled()
        {
            AddReward(-1f);
            cumulativeReward = GetCumulativeReward();
            EndEpisode();
        }
        
        private IEnumerator FlashGround(Color targetColor, float duration)
        {
            float elapsedTime = 0f;
        
            groundRenderer.material.color = targetColor;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                groundRenderer.material.color = Color.Lerp(targetColor, _defaultGroundColor, elapsedTime / duration);
                yield return null;
            }
        }

        public override void OnEpisodeBegin()
        {
            if (groundRenderer && cumulativeReward != 0f)
            { 
                Color flashColor = (cumulativeReward > 0f) ? Color.green : Color.red;

                if (_flashGroundCoroutine != null)
                {
                    StopCoroutine(_flashGroundCoroutine);                    
                }

                _flashGroundCoroutine = StartCoroutine(FlashGround(flashColor, 1.0f));
            }
            
            currentEpisode++;
            cumulativeReward = 0f;

            SpawnPlayer();
        }
        
        private void SpawnPlayer()
        {
            transform.localRotation = Quaternion.identity;
            transform.localPosition = new Vector3(0f, 0.5f, 0f);
            var location = new Vector3(0f, 0.5f, 0f);
            
            
            // Randomize the direction on the Y-axis (angle in degrees)
            float randomAngle = Random.Range(0f, 360f);
            Vector3 randomDirection = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;
        
            // Randomize the distance within range [1, 2.5]
            float randomDistance = Random.Range(1f, 10f);
        
            // Calculate the player's position
            Vector3 playerPosition = location + randomDirection * randomDistance;
        
            // Apply the calculated position to the player
            
            Instantiate(target, playerPosition, Quaternion.identity);
        }

        
    }
}