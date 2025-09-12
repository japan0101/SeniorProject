using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesScript.Melee
{
    public class MeleeEnemyAttack:EnemyAttack
    {
        [Header("Melee config")]
        public float attack_speed = 1440f;

        public override void OnAttack()
        {
            Debug.Log("Slashing");
        }
        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player")) {
                isMissed = false;//For using in OnDestroyed checks weather the attack hit a player
            }
        }
        void Update()
        {
            transform.RotateAround(transform.position, transform.up, attack_speed * Time.deltaTime/4f);
        }
    }
}