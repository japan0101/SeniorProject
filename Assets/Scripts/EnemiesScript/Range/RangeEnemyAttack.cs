using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesScript.Range
{
    public class RangeEnemyAttack:EnemyAttack
    {
        [Header("Range config")]
        public float power;
        //public float attack_speed = 1440f;

        //public override void OnAttack(GameObject attacker)
        //{
        //    if (_atk) return;
        //    slash.GetComponent<EnemyMeleeAttack>().attacker = attacker;
        //    _atk = Instantiate(slash, attacker.transform); //create attack hitbox
        //    Destroy(_atk, 0.5f); //despawn after a certain time
        //}

        void Update()
        {
            
        }
    }
}