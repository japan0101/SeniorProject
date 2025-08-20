using UnityEngine;

namespace EnemiesScript
{
    public abstract class EnemyAttack : MonoBehaviour
    {
        public GameObject attacker;
        public abstract void OnAttack(GameObject attacker);
    }
}