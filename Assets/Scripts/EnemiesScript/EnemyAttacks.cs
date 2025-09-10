using UnityEngine;
using System;

namespace EnemiesScript
{
    public abstract class EnemyAttack : MonoBehaviour
    {
        public GameObject attacker;
        [Header("Base Config")]
        public float damage;
        public float lifetime;
        public float baseKnockbackForce;
        protected bool isMissed = true;
        public event Action OnMissed;
        public abstract void OnAttack();

        public void OnDestroy()
        {
            if (isMissed)
            {
                Debug.Log("Missed From Attacks");
                OnMissed?.Invoke();
            }
        }
    }
}