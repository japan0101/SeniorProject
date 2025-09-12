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
            if (isMissed)// check weather the launced attack have hit an opponent, the logic of this should be done in a child component since some attack might have AOE
            {
                OnMissed?.Invoke(); // tell other component that is listening that an attack has missed
                //Debug.Log("Missed From Attacks");
            }
        }
    }
}