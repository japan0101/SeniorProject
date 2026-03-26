using System.Collections;
using UnityEngine;

namespace EnemiesScript.Boss
{
    public class BossJSlamAttack : EnemyAttack
    {
        [Header("Specific config")] public float hold_duration = 0.7f;

        public float attack_window = 0.3f;
        public GameObject effect;
        protected Coroutine attackRoutine;

        protected float holdposeTimer = 0;

        private void Update()
        {
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                isMissed = false; //For using in OnDestroyed checks weather the attack hit a player
            }
            else if (other.gameObject.layer == 10)
            {
                animator.SetTrigger("EndAttack");
                Destroy(gameObject, lifetime);
                // Use a local variable — do NOT overwrite 'effect' or next call will Instantiate a destroyed object
                if (effect != null)
                {
                    var spawnedEffect = Instantiate(effect, transform.position, Quaternion.identity);
                    Destroy(spawnedEffect, 0.5f);
                }
                GetComponent<MeshCollider>().enabled = true;
            }
        }

        public override void OnAttack(float dmgModifier)
        {
            damage = damage * dmgModifier;
            transform.position = new Vector3(transform.position.x, transform.position.y - 1.5f, transform.position.z);
            attackRoutine = StartCoroutine(AttackSequence(hold_duration));
        }

        public override void OnAttack(float dmgModifier, Vector3 direction)
        {
            OnAttack(dmgModifier);
        }

        private IEnumerator AttackSequence(float delayTime)
        {
            Debug.Log("Holding Pose");
            GetComponentInParent<Rigidbody>().useGravity = false;
            GetComponentInParent<Rigidbody>().AddForce(transform.up * 800);
            animator.SetTrigger("Jump Holdpose");
            yield return new WaitForSeconds(delayTime);
            animator.SetTrigger("Jump");
            attackRoutine = null;
            GetComponent<BoxCollider>().enabled = true;
            GetComponentInParent<Rigidbody>().useGravity = true;
            Debug.Log("Hit");
        }
    }
}