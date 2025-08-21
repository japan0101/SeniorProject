using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesScript.Melee
{
    public class MeleeEnemyAttack:EnemyAttack
    {
        [Header("Melee config")]
        public float attack_speed = 1440f;

        //public override void OnAttack(GameObject attacker)
        //{
        //    if (_atk) return;
        //    slash.GetComponent<EnemyMeleeAttack>().attacker = attacker;
        //    _atk = Instantiate(slash, attacker.transform); //create attack hitbox
        //    Destroy(_atk, 0.5f); //despawn after a certain time
        //}

        void Update()
        {
            transform.RotateAround(transform.position, transform.up, attack_speed * Time.deltaTime/4f);
        }
    }
}