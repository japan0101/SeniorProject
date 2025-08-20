using System.Collections.Generic;
using UnityEngine;

namespace EnemiesScript
{
    public abstract class Enemy : MonoBehaviour
    {
        [Header("Enemy Data")]
        public float hp;
        public float maxHp;
        public List<EnemyAttack> attacks = new List<EnemyAttack>();
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (!(hp <= 0)) return;
            Destroy(this.gameObject);
        }

        private void Attack()
        {
            
        }

        private void OnHit(GameObject Other)
        {
            
        }

        private void TakeDamage(float damage)
        {
            hp -= damage;
        }

        private void OnKilled(GameObject Other)
        {
            
        }
        
    }
}
