using System;
using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


namespace AutoPlayerScript
{
    public class AutoPlayerAgent:Agent
    {
        [SerializeField] private GameObject targetPrefab;
        [SerializeField] private Renderer groundRenderer;
        private TrainerNavigation agent;
        [HideInInspector] public int currentEpisode;
        [HideInInspector] public float cumulativeReward;
        private AutoShoot autoShoot;
        [SerializeField] private GameObject environment;
        public bool isTraining;
        private PlayerHealth _playerHealthManager;
        private GameObject _enemy1;
        private GameObject _enemy2;
        public TrainingController arenaController;
        
        float Timer = 0;//for testing agent action remove later

        new void Awake()
        {
            base.Awake();
            autoShoot = GetComponent<AutoShoot>();
            agent = GetComponent<TrainerNavigation>();
            _playerHealthManager = GetComponent<PlayerHealth>();
        }
        public override void Initialize()
        {
            if (agent == null) Debug.LogError($"{name}: TrainerNavigation not found on {gameObject}");
            if (isTraining)
            {
                TrainerManager.OnBulletHitEnemy += OnAttackLanded;
                TrainerManager.OnBulletMiss += OnMissed;
                TrainerManager.OnKillEnemy += OnKilledTarget;
                _playerHealthManager.OnPlayerDie += OnKilled;
                _playerHealthManager.OnPlayerHurt += OnHurt;
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
            //discreteActionsOut[1] = 0; // Do nothing


            //if (Input.GetKey(KeyCode.LeftShift))
            //{
            //    discreteActionsOut[0] = 1;//Dash
            //}
            if (Input.GetKey(KeyCode.Space))
            {
                discreteActionsOut[0] = 1;
            }
            if (Input.GetKey(KeyCode.R))
            {
                Debug.Log("Reloading");
                discreteActionsOut[0] = 2;
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
            base.CollectObservations(sensor);
            sensor.AddObservation(autoShoot.GetCurrentAmmo());
        }
        public override void OnActionReceived(ActionBuffers actions)
        {
            
            var moveX = actions.ContinuousActions[0];
            var moveZ = actions.ContinuousActions[1];
            var rotation = actions.ContinuousActions[2];
            //var special = actions.DiscreteActions[0];
            //var attack = actions.DiscreteActions[1];

            agent.MoveAgentX(moveX);
            agent.MoveAgentZ(moveZ);
            agent.RotateAgent(rotation);
            switch (actions.DiscreteActions[0])
            {
                case 1:
                    if (autoShoot.CanShoot())
                    {
                        autoShoot.ShootWeapon();
                        OnAttack();
                    }
                    break;
                case 2:
                    if(autoShoot.canReload())
                    {
                        autoShoot.StartReload();
                    }
                    autoShoot.ReloadWeapon();
                    break;
            }
            base.OnActionReceived(actions);
            
            float minSafeDistance = 3.0f;   // too close if closer than this
            float idealCombatDistance = 5.0f; // encourage hovering around this range
            
            if (!isTraining) return;
            if (_enemy1 != null)
            {
                HandleSpacingReward(_enemy1.transform, minSafeDistance, idealCombatDistance);
            }

            if (_enemy2 != null)
            {
                HandleSpacingReward(_enemy2.transform, minSafeDistance, idealCombatDistance);
            }
            
            // Penalty given each step to encourage agent to finish a task quickly
            AddReward(-0.0001f);
            // Survival incentive
            // AddReward(0.0001f);
            // // Update the cumulative reward after adding the step penalty.
            cumulativeReward = GetCumulativeReward();
            
        }
        
        private void HandleSpacingReward(Transform enemy, float minSafeDistance, float idealDistance)
        {
            float currentDistance = Vector3.Distance(transform.position, enemy.position);

            // Penalize being too close (inside min safe distance)
            if (currentDistance < minSafeDistance)
            {
                AddReward(-0.002f);
            }

            // Small reward for being near the "ideal combat distance"
            float rangeScore = 1f - Mathf.Abs(currentDistance - idealDistance) / idealDistance;
            AddReward(rangeScore * 0.001f);

            // Optional: reward moving closer (not circling forever)
            // float distanceDelta = lastDistance1 - currentDistance;
            // AddReward(Mathf.Clamp(distanceDelta * 0.01f, -0.01f, 0.01f));
        }

        public void OnAttack()
        {
            if (!isTraining) return;
            // AddReward(-0.02f);
            // cumulativeReward = GetCumulativeReward();
        }
        public void OnMissed(AutoPlayerAgent shooter)
        {
            if (!isTraining || shooter != this) return;
            //Debug.Log("Autoplayer missed attacks");
            //AddReward(-0.02f);
            //cumulativeReward = GetCumulativeReward();
        }

        public void OnSpecial()
        {
            if (!isTraining) return;
            AddReward(-0.01f);
            cumulativeReward = GetCumulativeReward();
        }
       
        public void OnAttackLanded(AutoPlayerAgent shooter)// Called when Agent Hit Something
        {
            if (!isTraining || shooter != this) return;
            Debug.Log("Autoplayer landed attacks");
            AddReward(0.05f);
            cumulativeReward = GetCumulativeReward();
        }
        public void OnKilledTarget(AutoPlayerAgent shooter)// Called when Agent Kill Something
        {
            if (!isTraining || shooter != this) return;
            AddReward(5f);
            cumulativeReward = GetCumulativeReward();
        }
        public void OnKilled()
        {
            // Getting Killed
            if (!isTraining) return;
            AddReward(-1f);
            cumulativeReward = GetCumulativeReward();
            // arenaController?.PlayerDefeated(this);
        }
        
        public void OnHurt()
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
            
            // The Training Manager Handle the Spawning Logic
            // SpawnEnemy();
        }
        
        private void SpawnEnemy()
        {
            var localOrigin = new Vector3(0f, 0.5f, 0f);
            
            
            // Randomize the direction on the Y-axis (angle in degrees)
            float randomAngle1 = Random.Range(0f, 360f);
            Vector3 randomDirection1 = Quaternion.Euler(0f, randomAngle1, 0f) * Vector3.forward;
            
            float randomAngle2 = Random.Range(0f, 360f);
            Vector3 randomDirection2 = Quaternion.Euler(0f, randomAngle2, 0f) * Vector3.forward;
        
            // Randomize the distance within range [1, 2.5]
            float randomDistance1 = Random.Range(1f, 10f);
            float randomDistance2 = Random.Range(1f, 10f);
        
            // Calculate the player's position
            Vector3 localEnemyPosition1 = localOrigin + randomDirection1 * randomDistance1;
            Vector3 localEnemyPosition2 = localOrigin + randomDirection2 * randomDistance2;
        
            // Apply the calculated position to the player
            
            if (_enemy1) Destroy(_enemy1);
            if (_enemy2) Destroy(_enemy2);
            
            _enemy1 = Instantiate(targetPrefab, environment.transform);
            _enemy1.transform.localPosition = localEnemyPosition1;

            _enemy2 = Instantiate(targetPrefab, environment.transform);
            _enemy2.transform.localPosition = localEnemyPosition2;
        }
    }
}