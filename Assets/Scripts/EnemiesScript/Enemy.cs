using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements.Experimental;

namespace EnemiesScript
{
    public abstract class Enemy : MonoBehaviour
    {
        [Header("Enemy Data")]
        public float hp;
        public float maxHp;
        [FormerlySerializedAs("moveSpeed")] public float baseSpeed;
        public float dashSpeed;
        public float dashCooldown;
        public float dashDuration;
        public float dashConsume;
        public float rotateSpeed;
        public float defence;
        public float energy;
        public float maxEnergy;
        public float energyRegenPerSecond;
        
        protected float dashTimer = 0;
        protected float realSpeed;
        private bool isDashing = false;
        public List<EnemyAttack> attacks = new List<EnemyAttack>();
        protected Rigidbody rb;
        public PlayerHealth _playerHealthManager;
        public GameObject _player;
        private bool attacking = false;
        private bool missed = false;

        // Start is called once before the first execution of Update after the MonoBehaviour is created

        protected void Awake()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            realSpeed = baseSpeed;
        }

        // Update is called once per frame
        protected void Update()
        {
            if (isDashing) {
                if (dashTimer > dashDuration)//check for dash cooldown when is dashing
                {
                    isDashing = false;
                    dashTimer = 0;
                }
                dashTimer += Time.deltaTime;
            }
            RegenEnergy(energyRegenPerSecond);
            // Debug.Log(realSpeed);
        }

        public void AddListenerToTarget(GameObject _target)
        {
            _playerHealthManager = _target.gameObject.GetComponent<PlayerHealth>();
            _playerHealthManager.OnPlayerHurt += OnAttackLanded;
            _playerHealthManager.OnPlayerDie += OnKilledTarget;
        }
        public abstract void Specials(int actionIndex);
        public abstract void MoveAgentX(float actionValue);
        public abstract void MoveAgentZ(float actionValue);
        public abstract void RotateAgent(float actionValue);
        public abstract void Attack(int atkIndex);
        private void RegenEnergy(float amountPerSecond)
        {
            energy = Mathf.Clamp(energy + amountPerSecond * Time.deltaTime , 0, maxEnergy);
        }
        public void Dash()
        {
            isDashing = true;
            energy -= dashConsume;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == 9)
            {
                MainBullet bullet = other.gameObject.GetComponent<MainBullet>();
                if (bullet != null)
                {
                    //spawn hit FX then register damage
                    TakeDamage(bullet.damage);
                    OnHurt();
                }
            }
        }
        protected abstract void OnHurt(); // Called when Agent getting Hurt
        protected abstract void OnKilled(); // Called when Agent getting Killed
        protected abstract void OnAttackLanded(); // Called when Agent Hit Something
        protected abstract void OnKilledTarget(); // Called when Agent Kill Something

        protected void TakeDamage(float damage)
        {
            //instantiate damage number with DamageTextSpawner component
            //realDmg = 
            GetComponent<DamageTextSpawner>().SpawnDamageText(new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z), damage);
            hp -= damage - (defence / 100);// reduce health
        }


        protected void SpeedControl()
        {
            rb.linearDamping = 2.5f;
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (isDashing)
            {
                realSpeed = dashSpeed;
            }
            else
            {
                realSpeed = baseSpeed;
            }
            if (flatVel.magnitude > realSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * baseSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
            //Debug.Log(realSpeed);
        }
    }
}
