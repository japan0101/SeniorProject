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
        public float rotateSpeed;
        public List<EnemyAttack> attacks = new List<EnemyAttack>();
        protected Rigidbody rb;
        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            CheckDeath();
        }

        protected abstract void MoveAgent(int actionIndex);
        protected abstract void RotateAgent(int actionIndex);
        protected abstract void Attack(int atkIndex);

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
            GetComponent<DamageTextSpawner>().SpawnDamageText(new Vector3(transform.position.x, transform.position.y + 5, transform.position.z), damage);
            hp -= damage;// reduce health
        }

        protected abstract void OnKilled(GameObject other);
        protected void SpeedControl()
        {
            Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            }
        }
    }
}
