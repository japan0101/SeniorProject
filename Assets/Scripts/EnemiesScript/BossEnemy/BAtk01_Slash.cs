using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesScript.Boss
{
    public class BossSlashAttack:EnemyAttack
    {
        [Header("Specific config")]
        public float hold_duration = 0.7f;
        public float attack_window = 0.7f;

        protected float holdposeTimer = 0;
        protected Coroutine attackRoutine;
        public override void OnAttack(float dmgModifier)
        {
            damage = damage * dmgModifier;
            animator.SetTrigger("BasicSlash Holdpose");
            attackRoutine = StartCoroutine(AttackSequence(hold_duration));
        }
        public override void OnAttack(float dmgModifier, Vector3 direction)
        {
            OnAttack(dmgModifier);
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
        IEnumerator AttackSequence(float delayTime)
        {
            Debug.Log("Holding Pose");
            //The animation have a .28 seconds delay until the slaash happens
            yield return new WaitForSeconds(delayTime-0.28f);
            animator.SetTrigger("BasicSlash Start");
            yield return new WaitForSeconds(0.28f);
            attackRoutine = null;
            GetComponent<BoxCollider>().enabled = true;
            Debug.Log("Hit");
        }
    }
}