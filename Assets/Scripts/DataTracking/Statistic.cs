using UnityEngine;

[CreateAssetMenu(fileName = "Statistic", menuName = "Scriptable Objects/Statistic")]
public class Statistic : ScriptableObject
{
    public float EnemyDamageDealt;
    public float PlayerDamageDealt;
    public float GameTime;
    public int EnemyAttackCount;
    public int EnemyAttackHit;
    public float EnemyAttackAccuracy;
    public bool isPlayerWin;

    public void Reset()
    {
        EnemyDamageDealt = 0;
        PlayerDamageDealt = 0;
        GameTime = 0;
        EnemyAttackCount = 0;
        EnemyAttackHit = 0;
        EnemyAttackAccuracy = 0;
        isPlayerWin = false;
    }
}