using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesScript.Boss
{
    public class BossBodySlamAttack:EnemyAttack
    {
        [Header("Specific config")]
        public float hold_duration = 0.7f;
        public float attack_window = 0.3f;
        public GameObject effect;

        protected float holdposeTimer = 0;
        protected Coroutine attackRoutine;
        public void OnDestroy()
        {
            base.OnDestroy();
            animator.SetTrigger("EndAttack");
            Debug.Log("Return Friction to default");
            GetComponentInParent<Enemy>().groundFriction = 2.5f;
            GetComponentInParent<Enemy>().BslamOverride();
        }
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
        void Update()
        {
            
        }
        IEnumerator AttackSequence(float delayTime)
        {
            Debug.Log("Holding Pose");
            GetComponentInParent<Enemy>().groundFriction = 0.002f;
            animator.SetTrigger("BSlam Holdpose");
            yield return new WaitForSeconds(delayTime);//Hold pose
            animator.SetTrigger("Bslam Start");
            attackRoutine = null;
            //Start attacking after this
            GetComponent<BoxCollider>().enabled = true;
            GetComponentInParent<Rigidbody>().AddForce(transform.forward * 1000, ForceMode.Force);
            Debug.Log("Hit");
        }
    }
}