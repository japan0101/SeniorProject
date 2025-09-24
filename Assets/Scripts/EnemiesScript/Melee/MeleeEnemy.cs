using UnityEngine;

namespace EnemiesScript.Melee
{
    public class MeleeEnemy:Enemy
    {
        private EnemyAttack _atk;
        float Timer = 0;
        private MeleeEnemyAgent _agent;
        
        private void Awake()
        {
            _agent = GetComponent<MeleeEnemyAgent>();
        }
        
        public override void Attack(int atkIndex)
        {   
            if (_atk) return;
            if (_agent.isTraining)
            {
                _agent.OnAttack();
            }

            if (energy >= 10f)
            {
                energy -= 10f;
                _atk = Instantiate(attacks[atkIndex], gameObject.transform);
                _atk.OnMissed += miss;//Add method of missed attack aknowledgement to an event listener of the launched attacks
                Destroy(_atk.gameObject, _atk.lifetime);
            }
        }

        protected override void OnHurt()
        {
            if (_agent.isTraining)
            {
                _agent.OnHurt();
            }
        }

        protected override void OnKilled()
        {
            if (_agent.isTraining)
            {
                _agent.OnKilled();
            }
        }

        protected override void OnAttackLanded()
        {
            if (_agent.isTraining)
            {
                _agent.OnAttackLanded();
            }
        }

        protected override void OnKilledTarget()
        {
            if (_agent.isTraining)
            {
                _agent.OnKilledTarget();
            }
        }
        public void miss()//used to acknowledge that an attack launched by this agent has missed
        {
            Debug.Log("Enemy component acknowledge misses");
            _agent.OnAttackMissed();
        }
        
        public override void Specials(int actionIndex)
        {
            switch (actionIndex)
            {
                case 1:
                    if(energy >= dashConsume)
                    {
                        Dash();//increase speed of the agent for a duration and consume energy
                        _agent.OnSpecial();
                    }
                    break;
            }
        }

        public override void MoveAgentX(float actionValue)
        {
            SpeedControl();
            rb.AddForce(transform.right * realSpeed * actionValue * baseSpeed, ForceMode.Force);
        }

        public override void MoveAgentZ(float actionValue)
        {
            SpeedControl();
            rb.AddForce(transform.forward * realSpeed * actionValue * baseSpeed, ForceMode.Force);
        }
        
        public override void RotateAgent(float actionValue)
        {
            transform.Rotate(0f, rotateSpeed * actionValue * Time.deltaTime, 0f);
        }

        private void Update()
        {
            base.Update();
            if (hp <= 0)
            {
                if (_agent.isTraining)
                {
                    OnKilled();
                    hp = maxHp;
                    return;
                }
                Destroy(gameObject);
            }
        }
    }
}