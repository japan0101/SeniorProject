using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace EnemiesScript.Boss
{
    public class BossJSlamAttack:EnemyAttack
    {
        [Header("Specific config")]
        public float hold_duration = 0.7f;
        public float attack_window = 0.3f;
        public GameObject effect;

        protected float holdposeTimer = 0;
        protected Coroutine attackRoutine;
        public override void OnAttack(float dmgModifier)
        {
            damage = damage * dmgModifier;
            transform.position = new Vector3 (transform.position.x, transform.position.y -1.5f, transform.position.z);
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
            }else if(other.gameObject.layer == 10)
            {
                Destroy(gameObject, lifetime);
                effect = Instantiate(effect, transform.position, Quaternion.identity);
                Destroy(effect, 0.5f);
                GetComponent<MeshCollider>().enabled = true;
            }
        }
        void Update()
        {
            
        }
        
        IEnumerator AttackSequence(float delayTime)
        {
            Debug.Log("Holding Pose");
            GetComponentInParent<Rigidbody>().useGravity = false;
            GetComponentInParent<Rigidbody>().AddForce(transform.up*800);
            yield return new WaitForSeconds(delayTime);
            attackRoutine = null;
            GetComponent<BoxCollider>().enabled = true;
            GetComponentInParent<Rigidbody>().useGravity = true;
            Debug.Log("Hit");
        }
    }
}