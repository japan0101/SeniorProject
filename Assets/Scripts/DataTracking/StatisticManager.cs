using EnemiesScript;
using UnityEngine;

public class StatisticManager : MonoBehaviour
{
    [SerializeField] public Statistic currentStats;
    [SerializeField] public Enemy enemy;
    [SerializeField] public PlayerShoot playerShoot;
    [SerializeField] public PlayerHealth playerHealth;
    float matchStarts;

    private void Awake()
    {
        matchStarts = Time.time;

        enemy.TrackOnDeath.AddListener(OnGameEnd);
        enemy.TrackOnEnemyAttack.AddListener(OnEnemyAttack);
        enemy.TrackOnPlayerDamageDealt.AddListener(OnPlayerDamageDealt);

        playerHealth.TrackOnEnemyDamageDealt.AddListener(OnEnemyDamageDealt);
        playerHealth.TrackDeath.AddListener(OnGameEnd);
    }

    public void OnEnemyDamageDealt(float Amount)
    {
        currentStats.EnemyDamageDealt += Amount;
        OnEnemyAttackHits();
    }
    public void OnPlayerDamageDealt(float Amount)
    {
        currentStats.PlayerDamageDealt += Amount;
    }
    public void OnEnemyAttack()
    {
        currentStats.EnemyAttackCount++;
    }
    public void OnEnemyAttackHits()
    {
        currentStats.EnemyAttackHit++;
    }
    public void OnGameEnd(bool playerWin)
    {
        currentStats.GameTime = Time.time - matchStarts;
        currentStats.EnemyAttackAccuracy = currentStats.EnemyAttackHit / currentStats.EnemyAttackCount;
        currentStats.isPlayerWin = playerWin;
    }
}
