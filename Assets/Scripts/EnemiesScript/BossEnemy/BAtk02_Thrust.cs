using System.Collections;
using UnityEngine;

namespace EnemiesScript.Boss
{
    public class BossThrustAttack : EnemyAttack
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
                isMissed = false; //For using in OnDestroyed checks weather the attack hit a player
        }

        public override void OnAttack(float dmgModifier)
        {
            damage = damage * dmgModifier;
            animator.SetTrigger("Thrust Holdpose");
            attackRoutine = StartCoroutine(AttackSequence(hold_duration));
        }

        public override void OnAttack(float dmgModifier, Vector3 direction)
        {
            OnAttack(dmgModifier);
        }

        private IEnumerator AttackSequence(float delayTime)
        {
            Debug.Log("Holding Pose");
            //The animation have a .06 seconds delay until the slaash happens
            yield return new WaitForSeconds(delayTime - 0.06f);
            animator.SetTrigger("Thrust Start");
            yield return new WaitForSeconds(0.06f);
            attackRoutine = null;
            GetComponent<BoxCollider>().enabled = true;
            GetComponent<ParticleSystem>().Play();
            Debug.Log("Hit");
        }
    }
}