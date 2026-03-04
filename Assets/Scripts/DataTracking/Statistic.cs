using UnityEngine;

[CreateAssetMenu(fileName = "Statistic", menuName = "Scriptable Objects/Statistic")]
public class Statistic : ScriptableObject
{
    public float EnemyDamageDealt;
    public float PlayerDamageDealt;
    public float GameTime;
    public int EnemyAttackCount;
    public int EnemyAttackHit;
    public int EnemyAttackAccuracy;
}
