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
        [SerializeField] private TrainerNavigation agent;
        [HideInInspector] public int currentEpisode;
        [HideInInspector] public float cumulativeReward;
        [SerializeField] public AutoShoot autoShoot;
        private Transform arena;
        public bool isTraining;
        private Color _defaultGroundColor;
        private Coroutine _flashGroundCoroutine;
        private PlayerHealth _playerHealthManager;
        private GameObject _enemy1;
        private GameObject _enemy2;
        
        float Timer = 0;//for testing agent action remove later

        public override void Initialize()
        {
            if (isTraining)
            {
                TrainerManager.OnBulletHitEnemy += OnAttackLanded;
                TrainerManager.OnBulletMiss += OnMissed;
                TrainerManager.OnKillEnemy += OnKilledTarget;
                arena = this.transform.parent.gameObject.transform;
                currentEpisode = 0;
                cumulativeReward = 0f;
                if (groundRenderer)
                {
                    _defaultGroundColor = groundRenderer.material.color;
                }
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
                    autoShoot.ReloadWeapon();
                    break;
            }
            //agent.Specials(special);
            

            //if (attack > 0)
            //{
            //    agent.Attack(attack - 1);
            //    attackedThisStep = true;
            //}
            base.OnActionReceived(actions);
            
        }

        public void OnAttack()
        {
            if (!isTraining) return;
            AddReward(-0.02f);
            cumulativeReward = GetCumulativeReward();
        }
        public void OnMissed(AutoPlayerAgent shooter)
        {
            if (!isTraining || shooter != this) return;
            Debug.Log("Autoplayer missed attacks");
            AddReward(-0.02f);
            cumulativeReward = GetCumulativeReward();
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
            Debug.Log("Autoplayer killed a target");
            AddReward(1f);
            cumulativeReward = GetCumulativeReward();
            EndEpisode();
        }
        public void OnKilled()
        {
            // Getting Killed
            if (!isTraining) return;
            AddReward(-1f);
            cumulativeReward = GetCumulativeReward();
            EndEpisode();
        }
        
        public void OnHurt()
        {
            if (!isTraining) return;
            AddReward(-0.01f);
            cumulativeReward = GetCumulativeReward();
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
            if (!isTraining) return;
            if (!groundRenderer.Equals(null) && cumulativeReward != 0f)
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
            
            _enemy1 = Instantiate(targetPrefab, localEnemyPosition1, Quaternion.identity);
            _enemy2 = Instantiate(targetPrefab, localEnemyPosition2, Quaternion.identity);
            
        }
    }
}