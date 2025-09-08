using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.GraphicsBuffer;

namespace EnemiesScript.Range
{
    public class RangeEnemyAgent:Agent
    {
        [SerializeField] private GameObject target;
        [SerializeField] private Renderer groundRenderer;
        [SerializeField] private Enemy agent;
        [SerializeField] private float maxSpeed = 2f;
        
        [HideInInspector] public int currentEpisode;
        [HideInInspector] public float cumulativeReward;

        private Color _defaultGroundColor;
        private Coroutine _flashGroundCoroutine;
        private GameObject _target;
        private PlayerHealth _playerHealthManager;

        float Timer = 0;//for testing agent action remove later
        public override void Initialize()
        {
            currentEpisode = 0;
            cumulativeReward = 0f;

            if (_target)
            {
                _playerHealthManager = _target.gameObject.GetComponent<PlayerHealth>();
                _playerHealthManager.OnPlayerHurt += OnAttackLanded;
                _playerHealthManager.OnPlayerDie += OnPlayerDie;
            }

            if (groundRenderer)
            {
                _defaultGroundColor = groundRenderer.material.color;
            }
        }
        protected void OnAttackLanded()// Called when Agent Kill Something
        {
            Debug.Log("Get rewards for hurting player based on damage or something");
        }
        protected void OnPlayerDie()// Called when Agent Kill Something
        {
            Debug.Log("Get rewards for killing player");
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
            continuousActions[2] = Input.GetAxis("Mouse X"); // Shitty Control Who TF Write this

            discreteActionsOut[0] = 0; // Do nothing
            discreteActionsOut[1] = 0; // Do nothing


            if (Input.GetKey(KeyCode.LeftShift))
            {
                discreteActionsOut[0] = 1;//Dash
            }

            if (Input.GetMouseButton(0))
            {
                discreteActionsOut[1] = 1;
            }
        }
        public override void CollectObservations(VectorSensor sensor)
        {
            // Give Agent the information about the state
            sensor.AddObservation(transform.localPosition);
        }
        public override void OnActionReceived(ActionBuffers actions)
        {
            //transform.localPosition += new Vector3(xDistance, 0, zDistance);
            
            // Penalty given each step to encourage agent to finish a task quickly
            base.OnActionReceived(actions);
            var movement = actions.DiscreteActions[0];
            var rotation = actions.DiscreteActions[1];
            var attack = actions.DiscreteActions[2];

            // agent.MoveAgent(movement);
            agent.RotateAgent(rotation);
            if (attack > 0)
            {
                agent.Attack(attack - 1);
            }
        }
        private void MoveAgent(ActionSegment<int> act)
        {

            var movement = act[0];
            var rotation = act[1];
            var attack = act[2];
            Debug.Log(act);
            // agent.MoveAgent(movement);
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