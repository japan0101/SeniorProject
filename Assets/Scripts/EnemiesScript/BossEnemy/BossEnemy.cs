using AutoPlayerScript;
using UnityEngine;

namespace EnemiesScript.Boss
{
    public class BossEnemy : Enemy
    {
        private const int basicslashIndex = 0;
        private const int thrustIndex = 1;
        private const int warcryIndex = 2;
        private const int bodyslamIndex = 3;
        private const int jumpslamIndex = 4;
        private const int evadeslashIndex = 5;
        [SerializeField] private AutoPlayerAgent agent;
        [SerializeField] private bool isBehaviorGraph;
        private BossEnemyAgent _agent;
        private Animator animator;
        private EnemyAttack _atk;

        private new void Awake()
        {
            if (!isBehaviorGraph) _agent = GetComponent<BossEnemyAgent>();
            base.Awake();

            animator = GetComponentInChildren<Animator>();
        }

        private new void Update()
        {
            base.Update();
            if (hp <= 0)
            {
                OnKilled();
                TrackOnDeath?.Invoke(true);
                Destroy(gameObject);
            }

            energy = Mathf.Clamp(energy + energyRegenPerSecond * Time.deltaTime, 0, maxEnergy);
            if (_atk == null)
                animator.SetFloat("Speed", realSpeed);
            else
                animator.SetFloat("Speed", 0);

            Debug.DrawRay(transform.position, transform.forward * 5, Color.blue);
            Debug.DrawRay(transform.position, transform.right * 5, Color.pink);
            //Debug.Log($"{_atk == null} : {animator.GetFloat("Speed")}");
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 10);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 20);
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 30);
        }

        public bool isAttacking()
        {
            return _atk != null;
        }

        public override void Attack(int atkIndex)
        {
            if (_atk != null) return; // Prevent attacking if already attacking
            if (!isBehaviorGraph && _agent.isTraining) _agent.OnAttack();
            var actualIndex = atkIndex - 1;
            Debug.Log($"Attempting attack with input index: {atkIndex}, array index: {actualIndex}");

            // Failsafe: Ensure the index is valid so we don't get Array Out Of Bounds errors
            if (actualIndex < 0 || actualIndex >= attacks.Count || attacks[actualIndex] == null)
            {
                Debug.LogWarning("Invalid attack index passed to Attack()!");
                return;
            }

            // HP phase gate — hard failsafe in case action mask is bypassed
            // Uses atkIndex (1-based, same as BossEnemyAgent constants)
            float healthPct = hp / maxHp;
            if (atkIndex == 3 && healthPct > 0.75f) return; // warcry: needs < 75% HP
            if (atkIndex == 4 && healthPct > 0.50f) return; // bodyslam: needs < 50% HP
            if (atkIndex == 5 && healthPct > 0.25f) return; // jumpslam: needs < 25% HP
            if (atkIndex == 6 && healthPct > 0.25f) return; // evadeslash: needs < 25% HP

            // Core logic
            _atk = Instantiate(attacks[actualIndex], gameObject.transform);
            _atk.animator = animator;
            _atk.OnAttack(atkModifier);
            _atk.OnMissed += Miss;
            nextAvailableAttackTime[actualIndex] = Time.time + cooldownDurations[actualIndex] + _atk.lifetime;

            var autoDestroy = true;
            switch (actualIndex)
            {
                case warcryIndex:
                    bufftimeRoutine = StartCoroutine(BuffTimer(_atk.effectDuration));
                    break;

                case bodyslamIndex:
                    BslamOverride();
                    break;

                case jumpslamIndex:
                    // I noticed your original code didn't Destroy the Jump Slam. 
                    // If that was intentional, we set this to false!
                    autoDestroy = false;
                    break;

                case evadeslashIndex:
                    BslamOverride();
                    Debug.Log("Performing Evade Slash attack");
                    break;
            }

            if (autoDestroy) Destroy(_atk.gameObject, _atk.lifetime);
            _atk.OnMissed +=
                Miss; //Add method of missed attack aknowledgement to an event listener of the launched attacks
            TrackOnEnemyAttack?.Invoke();
        }

        protected override void OnHurt()
        {
            if (_agent != null)
                if (_agent.isTraining)
                    _agent.OnHurt();
        }

        protected override void OnKilled()
        {
            if (_agent != null)
                if (_agent.isTraining)
                {
                    TrainerManager.KillEnemy(agent);
                    base.OnKilled();
                    _agent.OnKilled();
                }
        }

        protected override void OnAttackLanded()
        {
            if (_agent != null)
                if (_agent.isTraining)
                    _agent.OnAttackLanded();
        }

        protected override void OnKilledTarget()
        {
            if (_agent != null)
                if (_agent.isTraining)
                    _agent.OnKilledTarget();
        }

        public void Miss() //used to acknowledge that an attack launched by this agent has missed
        {
            Debug.Log("Enemy component acknowledge misses");
            if (isBehaviorGraph) return;
            _agent.OnAttackMissed();
        }

        public override void Specials(int actionIndex)
        {
            switch (actionIndex)
            {
                case 1:
                    if (energy >= dashConsume)
                    {
                        Dash(); //increase speed of the agent for a duration and consume energy
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
    }
}