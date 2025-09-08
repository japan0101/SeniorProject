using Unity.MLAgents.Actuators;
using Unity.VisualScripting;
using UnityEngine;

namespace EnemiesScript.Melee
{
    public class MeleeEnemy:Enemy
    {
        private EnemyAttack _atk;
        float Timer = 0;
        private MeleeEnemyAgent _agent;
        private void Start()
        {
            base.Start();
            _agent = GetComponent<MeleeEnemyAgent>();
        }
        public override void Attack(int atkIndex)
        {   
            if (_atk) return;
            _atk = Instantiate(attacks[atkIndex], gameObject.transform);
            Destroy(_atk.gameObject, _atk.lifetime);
        }

        protected override void OnHurt(GameObject other)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnKilled(GameObject other)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnAttackLanded(GameObject other)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnKilledTarget(GameObject other)
        {
            throw new System.NotImplementedException();
        }


        public override void Specials(int actionIndex)
        {
            switch (actionIndex)
            {
                case 1:
                    if(energy >= dashConsume)
                    {
                        Dash();//increase speed of the agent for a duration and consume energy
                    }
                    break;
            }
        }

        public override void MoveAgentX(float actionValue)
        {
            rb.AddForce(transform.right * realSpeed * actionValue * baseSpeed, ForceMode.Force);
            SpeedControl();
        }

        public override void MoveAgentZ(float actionValue)
        {
            rb.AddForce(transform.forward * realSpeed * actionValue * baseSpeed, ForceMode.Force);
            SpeedControl();
        }
        
        
        public override void RotateAgent(float actionValue)
        {
            // switch (actionIndex)
            // {
            //     case 1: // Rotate left
            //         transform.Rotate(0f, -rotateSpeed * Time.deltaTime, 0f);
            //         break;
            //     case 2: // Rotate Right
            //         transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
            //         break;
            // }
            
            transform.Rotate(0f, rotateSpeed * actionValue * Time.deltaTime, 0f);
        }

        private void Update()
        {
            if (hp <= 0)
            {
                _agent.OnKilled();
                hp = maxHp;
            }
        }
    }
}