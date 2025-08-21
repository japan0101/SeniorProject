using UnityEngine;

namespace EnemiesScript
{
    public abstract class EnemyAttack : MonoBehaviour
    {
        public GameObject attacker;
        [Header("Base Config")]
        public float damage;
        public float lifetime;
        public float baseKnockbackForce;
        //public abstract void OnAttack(GameObject attacker);
    }
}