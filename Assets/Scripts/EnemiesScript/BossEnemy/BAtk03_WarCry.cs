using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesScript.Boss
{
    public class BossWCAttack:EnemyAttack
    {
        [Header("Specific config")]
        public float hold_duration = 0.7f;
        public float attack_window = 0.8f;

        protected float holdposeTimer = 0;
        protected Coroutine attackRoutine;
        public override void OnAttack(float dmgModifier)
        {
            damage = damage * dmgModifier;
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
        IEnumerator AttackSequence(float delayTime)
        {

            Debug.Log("Holding Pose");
            yield return new WaitForSeconds(delayTime);
            animator.SetTrigger("Shout");
            attackRoutine = null;
            GetComponent<SphereCollider>().enabled = true;
            Debug.Log("Hit");
        }
    }
}