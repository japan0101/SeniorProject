using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
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
        private void Update()
        {
            //for testing agent action remove later
            //agent.MoveAgent(Random.Range(0, 5));
            //if (Timer >= 2)
            //{
            //    agent.Attack(0);
            //    agent.RotateAgent(Random.Range(0, 3));
            //    Debug.Log("Attack");
            //    Timer = 0;
            //}
            //Timer += Time.deltaTime;
        }
        public override void Initialize()
        {
            currentEpisode = 0;
            cumulativeReward = 0f;

            if (groundRenderer)
            {
                _defaultGroundColor = groundRenderer.material.color;
            }
        }
        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var discreteActionsOut = actionsOut.DiscreteActions;

            discreteActionsOut[0] = 0; // Do nothing
            discreteActionsOut[1] = 0; // Do nothing
            discreteActionsOut[2] = 0; // Do nothing

            if (Input.GetKey(KeyCode.UpArrow))
            {
                discreteActionsOut[0] = 1;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                discreteActionsOut[0] = 2;
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                discreteActionsOut[0] = 3;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                discreteActionsOut[0] = 4;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                discreteActionsOut[1] = 1;
            }
            else if (Input.GetKey(KeyCode.A))
            {
                discreteActionsOut[1] = 2;
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                discreteActionsOut[2] = 1;
            }
            agent.MoveAgent(discreteActionsOut[0]);
            agent.RotateAgent(discreteActionsOut[1]);
            agent.Attack(discreteActionsOut[2]);
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
            // Agent decides what to do about the current state
            // Move the agent using the action
             MoveAgent(actions.DiscreteActions);

            // Penalty given each step to encourage agent to finish a task quickly
            AddReward(-2f/MaxStep);
            // Update the cumulative reward after adding the step penalty.
            cumulativeReward = GetCumulativeReward();
        }
        private void MoveAgent(ActionSegment<int> act)
        {

            var movement = act[0];
            var rotation = act[1];
            var attack = act[2];
            Debug.Log(act);
            agent.MoveAgent(movement);
            agent.RotateAgent(rotation);
            agent.Attack(attack);
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
        
            // Randomize the direction on the Y-axis (angle in degrees)
            float randomAngle = Random.Range(0f, 360f);
            Vector3 randomDirection = Quaternion.Euler(0f, randomAngle, 0f) * Vector3.forward;
        
            // Randomize the distance within range [1, 2.5]
            float randomDistance = Random.Range(1f, 10f);
        
            // Calculate the player's position
            Vector3 playerPosition = transform.localPosition + randomDirection * randomDistance;
        
            // Apply the calculated position to the player
            target.gameObject.transform.localPosition = new Vector3(playerPosition.x, 0.5f, playerPosition.z);
        }
    }
}