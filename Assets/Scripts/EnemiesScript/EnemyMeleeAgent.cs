using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEditor.Sprites;
using UnityEngine;
using UnityEngine.UIElements.Experimental;
using Random = UnityEngine.Random;

public class EnemyMeleeAgent : Agent
{
    [SerializeField] private Transform _player;
    [SerializeField] private Renderer _groundRenderer;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;

    private Renderer _renderer;

    [HideInInspector]public int CurrentEpisode = 0;
    [HideInInspector]public float CumulativeReward = 0f;
    
    private Color _defaultGroundColor;
    private Coroutine _flashGroundCoroutine;

    private Rigidbody rb;
    private BaseEnemy enemy;

    private BaseEnemy enemy;

    public override void Initialize()
    {
        Debug.Log("Initialize()");

        enemy = GetComponent<BaseEnemy>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; 
        _renderer = GetComponent<Renderer>();
        CurrentEpisode = 0;
        CumulativeReward = 0f;

        if (_groundRenderer != null)
        {   // Store default color of the ground
            _defaultGroundColor = _groundRenderer.material.color;
        }
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin()");
        
        if (_groundRenderer != null && CumulativeReward != 0f)
        {
            Color flashColor = (CumulativeReward > 0f) ? Color.green : Color.red;
            
            // Stop any existing FlashGround coroutine before starting a new one
            if (_flashGroundCoroutine != null)
            {
                StopCoroutine(_flashGroundCoroutine);
            }

            _flashGroundCoroutine = StartCoroutine(FlashGround(flashColor, 1.0f));
        }

        CurrentEpisode++;
        CumulativeReward = 0f;
        _renderer.material.color = Color.red;

        SpawnObjects();
    }

    private IEnumerator FlashGround(Color targetColor, float duration)
    {
        float elapsedTime = 0f;
        
        _groundRenderer.material.color = targetColor;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _groundRenderer.material.color = Color.Lerp(targetColor, _defaultGroundColor, elapsedTime / duration);
            yield return null;
        }
    }

    private void SpawnObjects()
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
        _player.localPosition = new Vector3(playerPosition.x, 0.5f, playerPosition.z);
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
        discreteActionsOut[1] = 0;

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
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > _moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * _moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Agent decides what to do about the current state
        // Move the agent using the action
        MoveAgent(actions.DiscreteActions);
        
        // Penalty given each step to encourage agent to finish task quickly
        AddReward(-2f / MaxStep);
        
        // Update the cumulative reward after adding the step penalty.
        CumulativeReward = GetCumulativeReward();
    }

    public void MoveAgent(ActionSegment<int> act)
    {   
        
        var movement = act[0];
        var rotation = act[1];
        var attack = act[2];
        
        switch (movement)
        {
            case 1: // Move forward
                rb.AddForce(transform.forward * _moveSpeed * 10f, ForceMode.Force);
                // transform.position += transform.forward * _moveSpeed * Time.deltaTime;
                break;
            case 2: // Move Backward
                rb.AddForce(-transform.forward * _moveSpeed * 10f, ForceMode.Force);
                // transform.position -= transform.forward * _moveSpeed * Time.deltaTime;
                break;
            case 3: // Stride Right
                rb.AddForce(transform.right * _moveSpeed * 10f, ForceMode.Force);
                break;
            case 4: // Stride Left
                rb.AddForce(-transform.right * _moveSpeed * 10f, ForceMode.Force);
                break;
        }

        switch (rotation)
        {
            case 1: // Rotate left
                transform.Rotate(0f, -_rotateSpeed * Time.deltaTime, 0f);
                break;
            case 2: // Rotate Right
                transform.Rotate(0f, _rotateSpeed * Time.deltaTime, 0f);
                break;
        }

        switch (attack)
        {
            case 1: // Basic Attack
                enemy.attacks[0].OnAttack();
                Debug.Log("Attack!");
                break;
        }
        SpeedControl();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerReached();
        }
    }

    private void PlayerReached()
    {
        AddReward(1.0f);
        CumulativeReward = GetCumulativeReward();
        
        EndEpisode();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            // Apply a small negative reward when the collision starts
            AddReward(-0.05f);
            
            // Change the color
            if (_renderer != null)
            {
                _renderer.material.color = Color.yellow;
            }
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            // Continually penalize
            AddReward(-0.01f * Time.fixedDeltaTime);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            if (_renderer != null)
            {
                _renderer.material.color = Color.red;
            }
        }
    }
}
