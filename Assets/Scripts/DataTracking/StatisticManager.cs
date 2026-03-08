using EnemiesScript.Boss;
using UnityEngine;

public class StatisticManager : MonoBehaviour
{
    [SerializeField] public Statistic currentStats;
    [SerializeField] public BossEnemy enemy;
    [SerializeField] public PlayerShoot playerShoot;
    [SerializeField] public PlayerHealth playerHealth;
    float matchStarts;

    private void Awake()
    {
        matchStarts = Time.time;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnEnemyDamageDealt(int Amount)
    {

    }
    public void OnPlayerDamageDealt(int Amount)
    {

    }
    public void OnEnemyAttack()
    {

    }
    public void OnEnemyAttackHits()
    {

    }
    public void OnGameEnd()
    {
        currentStats.GameTime = Time.time - matchStarts;
    }
}
