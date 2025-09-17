using System;
using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


namespace EnemiesScript.Melee
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
        private bool attackedThisStep = false;
        private float lastDistance;
        private float minAttackDistance = 1.5f;
        
        float Timer = 0;//for testing agent action remove later

        private void Awake()
        {
            if (!isTraining)
            {
                //agent._player = GameObject.FindGameObjectWithTag("Enemy");
            }
        }

        public override void Initialize()
        {
            if (isTraining)
            {
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
            sensor.AddObservation(transform.localPosition);
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
                    }
                    break;
            }
            //agent.Specials(special);
            
            attackedThisStep = false;

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

        public void OnSpecial()
        {
            if (!isTraining) return;
            AddReward(-0.01f);
            cumulativeReward = GetCumulativeReward();
        }
        public void OnAttackMissed()//Called by Enemy attack event listener to notify that the attack launched did not and on a player
        {
            if (!isTraining) return;
            
        }
        public void OnAttackLanded()// Called when Agent Hit Something
        {
            if (!isTraining) return;
            AddReward(0.05f);
            cumulativeReward = GetCumulativeReward();
        }
        public void OnKilledTarget()// Called when Agent Kill Something
        {
            if (!isTraining) return;
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
        }
        
        private void SpawnPlayer()
        {
        }
    }
}