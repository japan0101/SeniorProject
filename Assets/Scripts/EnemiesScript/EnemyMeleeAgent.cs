using System;
using System.Linq.Expressions;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class EnemyMeleeAgent : Agent
{
    [SerializeField] private Transform _player;
    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _rotateSpeed;

    private Renderer _renderer;

    private int _currentEpisode = 0;
    private float _cumulativeReward = 0f;

    public override void Initialize()
    {
        Debug.Log("Initialize()");

        _renderer = GetComponent<Renderer>();
        _currentEpisode = 0;
        _cumulativeReward = 0f;
    }

    public override void OnEpisodeBegin()
    {
        Debug.Log("OnEpisodeBegin()");

        _currentEpisode++;
        _cumulativeReward = 0f;
        _renderer.material.color = Color.red;

        SpawnObjects();
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
        Vector3 playerPosNormalized = _player.localPosition.normalized;

        // The Enemy's position
        Vector3 enemyPosNormalized = transform.localPosition.normalized;
        
        
        // The Enemy's direction (on the Y Axis)
        Vector3 forward = transform.forward;
        float enemyRotation = Mathf.Atan2(forward.x, forward.z) * Mathf.Rad2Deg;
        
        sensor.AddObservation(playerPosNormalized.x);
        sensor.AddObservation(playerPosNormalized.z);
        sensor.AddObservation(enemyPosNormalized.x);
        sensor.AddObservation(enemyPosNormalized.z);
        sensor.AddObservation(enemyRotation/180f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // Agent decides what to do about the current state
        Debug.Log("OnActionReceived()");
        // Move the agent using the action
        MoveAgent(actions.DiscreteActions);
        
        // Penalty given each step to encourage agent to finish task quickly
        AddReward(-2f / MaxStep);
        
        // Update the cumulative reward after adding the step penalty.
        _cumulativeReward = GetCumulativeReward();
    }

    public void MoveAgent(ActionSegment<int> act)
    {   
        Debug.Log("MoveAgent()");
        var action = act[0];

        switch (action)
        {
            case 1: // Move forward
                transform.position += transform.forward * _moveSpeed * Time.deltaTime;
                Debug.Log("Forward");
                break;
            case 2: // Rotate left
                transform.Rotate(0f, -_rotateSpeed * Time.deltaTime, 0f);
                Debug.Log("Left");
                break;
            case 3: // Rotate Right
                transform.Rotate(0f, _rotateSpeed * Time.deltaTime, 0f);
                Debug.Log("Right");
                break;
        }
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
        _cumulativeReward = GetCumulativeReward();
        
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
