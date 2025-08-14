using UnityEngine;

public abstract class BaseEnemyAttack : MonoBehaviour
{
    public GameObject attacker;
    public abstract void OnAttack(GameObject attacker);
}
