using EnemiesScript;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class StatisticManager : MonoBehaviour
{
    [SerializeField] public Statistic currentStats;
    [SerializeField] public Enemy enemy;
    [SerializeField] public PlayerShoot playerShoot;
    [SerializeField] public PlayerHealth playerHealth;
    [SerializeField] public TextMeshProUGUI resultPanel;
    float matchStarts;

    private void Awake()
    {
        currentStats.Reset();
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
        Debug.Log("Game Ending");
        currentStats.isPlayerWin = playerWin;
        DisplayResult();

        Debug.Log("Unlocking Mouse");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        currentStats.GameTime = Time.time - matchStarts;
        Time.timeScale = 0;
        currentStats.EnemyAttackAccuracy = currentStats.EnemyAttackCount != 0?currentStats.EnemyAttackHit / currentStats.EnemyAttackCount:0;
    }
    public void DisplayResult()
    {
        Debug.Log("Showing Results");
        resultPanel.SetText(
            $"Test Type A Result\n" +
            $"Enemy Damage Dealt: {currentStats.EnemyDamageDealt}\n" +
            $"Player Damage Dealt: {currentStats.PlayerDamageDealt}\n" +
            $"Game Time: {FormatTimeSpan(currentStats.GameTime)}\n" +
            $"Enemy Attack Count: {currentStats.EnemyAttackCount} \n" +
            $"Enemy Attack hit: {currentStats.EnemyAttackHit} \n" +
            $"Enemy Attack Accuracy: {currentStats.EnemyAttackAccuracy * 100}% \n" +
            $"Did Player Win: {currentStats.isPlayerWin} \n"
            );
        resultPanel.gameObject.SetActive(true);
    }
    public string FormatTimeSpan(float seconds)
    {
        TimeSpan t = TimeSpan.FromSeconds(seconds);

        return string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);

    }
}
