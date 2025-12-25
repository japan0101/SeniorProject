using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesScript.Boss
{
    public class BossSlashAttack:EnemyAttack
    {
        [Header("Specific config")]
        public float hold_duration = 0.3f;
        public float attack_window = 0.7f;

        public override void OnAttack()
        {
            Debug.Log("Slashing");
        }
        public override void OnAttack(Vector3 direction)
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
            
        }
        IEnumerator HoldPose(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
        }
    }
}