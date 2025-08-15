using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace EnemiesScript
{
    public class EnemyMeleeAgent : Agent
    {
        [FormerlySerializedAs("_player")] [SerializeField] private Transform player;
        [FormerlySerializedAs("_groundRenderer")] [SerializeField] private Renderer groundRenderer;
        [FormerlySerializedAs("_moveSpeed")] [SerializeField] private float moveSpeed;
        [FormerlySerializedAs("_rotateSpeed")] [SerializeField] private float rotateSpeed;

        private Renderer _renderer;

        [HideInInspector]public int currentEpisode;
        [HideInInspector]public float cumulativeReward;
    
        private Color _defaultGroundColor;
        private Coroutine _flashGroundCoroutine;

        private Rigidbody _rb;
        private BaseEnemy _enemy;

        public override void Initialize()
        {
            Debug.Log("Initialize()");

            _enemy = GetComponent<BaseEnemy>();
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true; 
            _renderer = GetComponent<Renderer>();
            currentEpisode = 0;
            cumulativeReward = 0f;

            if (groundRenderer != null)
            {   // Store default color of the ground
                _defaultGroundColor = groundRenderer.material.color;
            }
        }

        public override void OnEpisodeBegin()
        {
            //Debug.Log("OnEpisodeBegin()");
        
            if (groundRenderer != null && cumulativeReward != 0f)
            {
                Color flashColor = (cumulativeReward > 0f) ? Color.green : Color.red;
            
                // Stop any existing FlashGround coroutine before starting a new one
                if (_flashGroundCoroutine != null)
                {
                    StopCoroutine(_flashGroundCoroutine);
                }

                _flashGroundCoroutine = StartCoroutine(FlashGround(flashColor, 1.0f));
            }

            currentEpisode++;
            cumulativeReward = 0f;
            //_renderer.material.color = Color.red;

            SpawnPlayer();
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
            player.localPosition = new Vector3(playerPosition.x, 0.5f, playerPosition.z);
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
            } else if (Input.GetKey(KeyCode.RightArrow))
            {
                discreteActionsOut[0] = 3;
            }
            else if  (Input.GetKey(KeyCode.LeftArrow))
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
            } else if (Input.GetKey(KeyCode.Space))
            {
                discreteActionsOut[2] = 1;
            }
        }

        private void SpeedControl()
        {
            Vector3 flatVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                _rb.linearVelocity = new Vector3(limitedVel.x, _rb.linearVelocity.y, limitedVel.z);
            }
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            // Agent decides what to do about the current state
            // Move the agent using the action
            MoveAgent(actions.DiscreteActions);
        
            // Penalty given each step to encourage agent to finish a task quickly
            AddReward(-2f / MaxStep);
        
            // Update the cumulative reward after adding the step penalty.
            cumulativeReward = GetCumulativeReward();
        }

        private void MoveAgent(ActionSegment<int> act)
        {   
        
            var movement = act[0];
            var rotation = act[1];
            var attack = act[2];
        
            switch (movement)
            {
                case 1: // Move forward
                    _rb.AddForce(transform.forward * moveSpeed * 10f, ForceMode.Force);
                    // transform.position += transform.forward * _moveSpeed * Time.deltaTime;
                    break;
                case 2: // Move Backward
                    _rb.AddForce(-transform.forward * moveSpeed * 10f, ForceMode.Force);
                    // transform.position -= transform.forward * _moveSpeed * Time.deltaTime;
                    break;
                case 3: // Stride Right
                    _rb.AddForce(transform.right * moveSpeed * 10f, ForceMode.Force);
                    break;
                case 4: // Stride Left
                    _rb.AddForce(-transform.right * moveSpeed * 10f, ForceMode.Force);
                    break;
            }

            switch (rotation)
            {
                case 1: // Rotate left
                    transform.Rotate(0f, -rotateSpeed * Time.deltaTime, 0f);
                    break;
                case 2: // Rotate Right
                    transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
                    break;
            }

            switch (attack)
            {
                case 1: // Basic Attack
                    _enemy.attacks[0].OnAttack(gameObject);
                     //Debug.Log("Attack!");
                    break;
            }
            SpeedControl();
        }

        // private void OnTriggerEnter(Collider other)
        // {
        //     if (other.gameObject.CompareTag("Player"))
        //     {
        //         PlayerReached();
        //     }
        // }

        public void TakeDamage(float damage)
        {
            AddReward(-(0.05f * damage));
        }

        public void OnAttackSuccess()
        {
            Debug.Log("Agent sense hit");
            AddReward(0.1f);
        }

        public void OnKilled()
        {
            AddReward(0.5f);
            cumulativeReward = GetCumulativeReward();
            EndEpisode();
        }

        public void OnKillPlayer()
        {
            AddReward(1.0f);
            cumulativeReward = GetCumulativeReward();
            EndEpisode();
        }

        // private void PlayerReached()
        // {
        //     AddReward(1.0f);
        //     CumulativeReward = GetCumulativeReward();
        //     
        //     EndEpisode();
        // }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer != 9) return;
            // Change the color
            if (_renderer != null)
            {
                _renderer.material.color = Color.yellow;
            }
        }

        // private void OnCollisionStay(Collision other)
        // {
        //     if (other.gameObject.CompareTag("Wall"))
        //     {
        //         // Continually penalize
        //         AddReward(-0.01f * Time.fixedDeltaTime);
        //     }
        // }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.layer != 9) return;
        
            if (_renderer != null)
            {
                _renderer.material.color = Color.red;
            }
        }
    }
}
