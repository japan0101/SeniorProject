using AutoPlayerScript;
using UnityEngine;

namespace EnemiesScript.Boss
{
    public class BossEnemy:Enemy
    {
        private EnemyAttack _atk;
        private BossEnemyAgent _agent;
        [SerializeField]private AutoPlayerAgent agent;
        const int basicslashIndex = 0;
        const int thrustIndex = 1;
        const int warcryIndex = 2;
        const int bodyslamIndex = 3;
        const int jumpslamIndex = 4;
        const int evadeslashIndex = 5;
        private Animator animator;
        private new void Awake()
        {
            _agent = GetComponent<BossEnemyAgent>();
            base.Awake();

            animator = GetComponentInChildren<Animator>();
        }
        
        public override void Attack(int atkIndex)
        {   
            if (_atk) return;
            if (_agent.isTraining)
            {
                _agent.OnAttack();
            }
            switch (atkIndex-1)
            {
                case basicslashIndex://basic slash
                    _atk = Instantiate(attacks[basicslashIndex], gameObject.transform);
                    _atk.animator = animator;
                    _atk.OnAttack(atkModifier);
                    Destroy(_atk.gameObject, _atk.lifetime);
                    break;
                case thrustIndex://thrust
                    _atk = Instantiate(attacks[thrustIndex], gameObject.transform);
                    _atk.animator = animator;
                    _atk.OnAttack(atkModifier);
                    Destroy(_atk.gameObject, _atk.lifetime);
                    break;
                case warcryIndex://war cry
                    _atk = Instantiate(attacks[warcryIndex], gameObject.transform);
                    _atk.animator = animator;
                    _atk.OnAttack(atkModifier);
                    bufftimeRoutine = StartCoroutine(BuffTimer(_atk.effectDuration));
                    Destroy(_atk.gameObject, _atk.lifetime);
                    break;
                case bodyslamIndex://body slam
                    _atk = Instantiate(attacks[bodyslamIndex], gameObject.transform);
                    _atk.OnAttack(atkModifier);
                    Destroy(_atk.gameObject, _atk.lifetime);
                    BslamOverride();
                    Debug.Log("Perfoming Body Slam attack");
                    break;
                case jumpslamIndex://jump slam
                    _atk = Instantiate(attacks[jumpslamIndex], gameObject.transform);
                    _atk.animator = animator;
                    _atk.OnAttack(atkModifier);
                    Debug.Log("Perfoming Jump Slam attack");
                    break;
                case evadeslashIndex://evade slash
                    _atk = Instantiate(attacks[evadeslashIndex], gameObject.transform);
                    _atk.OnAttack(atkModifier);
                    Destroy(_atk.gameObject, _atk.lifetime);
                    BslamOverride();
                    Debug.Log("Perfoming Evade Slash attack");
                    break;
                default:
                    break;
            }
            //if (energy >= 10f)
            //{
            //    energy -= 10f;
            //    _atk = Instantiate(attacks[atkIndex], gameObject.transform);
                
                _atk.OnMissed += Miss;//Add method of missed attack aknowledgement to an event listener of the launched attacks
                
            //}
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
                TrainerManager.KillEnemy(agent);
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
        public void Miss()//used to acknowledge that an attack launched by this agent has missed
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

        private new void Update()
        {
            base.Update();
            if (hp <= 0)
            {
                OnKilled();
                Destroy(gameObject);
            }
            energy = Mathf.Clamp(energy + (energyRegenPerSecond * Time.deltaTime), 0, maxEnergy);
        }
        
    }
}