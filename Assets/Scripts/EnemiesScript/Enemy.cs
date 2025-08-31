using System.Collections.Generic;
using Unity.MLAgents.Actuators;
using UnityEngine;

namespace EnemiesScript
{
    public abstract class Enemy : MonoBehaviour
    {
        [Header("Enemy Data")]
        public float hp;
        public float maxHp;
        public float moveSpeed;
        public float dashSpeed;
        public float dashCooldown;
        public float dashDuration;
        public float dashConsume;
        public float rotateSpeed;
        public float defence;
        public float energy;
        public float maxEnergy;

        protected float dashTimer = 0;
        protected float realSpeed;
        private bool isDashing = false;
        public List<EnemyAttack> attacks = new List<EnemyAttack>();
        protected Rigidbody rb;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
        }

        // Update is called once per frame
        void Update()
        {
            CheckDeath();
            if (isDashing) {
                if (dashTimer > dashDuration)//check for dash cooldown when is dashing
                {
                    isDashing = false;
                    dashTimer = 0;
                    realSpeed = moveSpeed;
                }
                dashTimer += Time.deltaTime;
            }
        }

        public abstract void MoveAgent(int actionIndex);
        public abstract void RotateAgent(int actionIndex);
        public abstract void Attack(int atkIndex);
        public void Dash()
        {
            isDashing = true;
            realSpeed = dashSpeed;
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
                }
            }
        }
        private void CheckDeath()
        {
            if (!(hp <= 0)) { return; }
            Destroy(this.gameObject);
        }
        protected abstract void OnHit(GameObject other);

        protected void TakeDamage(float damage)
        {
            //instantiate damage number with DamageTextSpawner component
            GetComponent<DamageTextSpawner>().SpawnDamageText(new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z), damage);
            hp -= damage;// reduce health
        }

        protected abstract void OnKilled(GameObject other);

        protected void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVel.magnitude > realSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * realSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }
}
