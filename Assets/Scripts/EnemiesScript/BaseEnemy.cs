using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnemiesScript
{
    public class BaseEnemy : MonoBehaviour
    {
        [Header("Enemy Data")]
        public float hp=100;
        public float maxHp=100;
        public List<BaseEnemyAttack> attacks = new List<BaseEnemyAttack>();
        private BaseEnemyAttack _atk;

        private EnemyMeleeAgent _agent;
        float testTimer = 0f;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // foreach (var atk in attacks)
            // {
            //     atk.OnAttack();//try to attack
            // }
            _agent = GetComponent<EnemyMeleeAgent>();
        }
        private void OnCollisionEnter(Collision collision)
        {
            // Check if collision is with PlayerAttack layer
            if (collision.gameObject.layer == 9)
            {
                MainBullet bullet = collision.gameObject.GetComponent<MainBullet>();
                if (bullet != null)
                {
                    //spawn hit FX then register damage
                    TakeDamage(collision.contacts[0].point, bullet.damage);
                }

            }
        }

        public void TakeDamage(Vector3 dmgPos, float damage)
        {
            GetComponent<DamageTextSpawner>().SpawnDamageText(dmgPos, damage); //instantiate damage number with DamageTextSpawner component
            hp -= damage;
            _agent.TakeDamage(damage);

        }
        public void TakeDamage(float damage)
        {
            GetComponent<DamageTextSpawner>().SpawnDamageText(new Vector3(transform.position.x, transform.position.y + 1, transform.position.z), damage); //instantiate damage number with DamageTextSpawner component
            hp -= damage;
            _agent.TakeDamage(damage);

        }

        // Update is called once per frame
        void Update()
        {
            if (testTimer >= 2)
            {
                Debug.Log("attacking");
                _atk = Instantiate(attacks[0], gameObject.transform);
                testTimer = 0;
            }
            testTimer += Time.deltaTime;
            if (!(hp <= 0)) return;
            //Spawn dead FX then delete object
            _agent.OnKilled();
            hp = maxHp;
            
        }
    }
}
