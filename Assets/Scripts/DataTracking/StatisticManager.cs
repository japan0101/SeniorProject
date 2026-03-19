using System;
using EnemiesScript;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatisticManager : MonoBehaviour
{
    [SerializeField] public Statistic currentStats;
    [SerializeField] public Enemy enemy;
    [SerializeField] public PlayerShoot playerShoot;
    [SerializeField] public PlayerHealth playerHealth;
    [SerializeField] public TextMeshProUGUI resultPanel;
    private bool gameEnded;
    private float matchStarts;

    private void Awake()
    {
        Time.timeScale = 1;
        gameEnded = false;
        currentStats.Reset();
        matchStarts = Time.time;

        enemy.TrackOnDeath.AddListener(OnGameEnd);
        enemy.TrackOnEnemyAttack.AddListener(OnEnemyAttack);
        enemy.TrackOnPlayerDamageDealt.AddListener(OnPlayerDamageDealt);

        playerHealth.TrackOnEnemyDamageDealt.AddListener(OnEnemyDamageDealt);
        playerHealth.TrackDeath.AddListener(OnGameEnd);
    }

    private void Update()
    {
        if (gameEnded && Input.GetKey(KeyCode.Escape)) SceneManager.LoadScene(0);
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
        if (currentStats.EnemyAttackCount == 0)
            currentStats.EnemyAttackAccuracy = 0;
        else
            currentStats.EnemyAttackAccuracy = currentStats.EnemyAttackHit * 100 / currentStats.EnemyAttackCount;
        gameEnded = true;
        Time.timeScale = 0;
    }

    public void DisplayResult()
    {
        Debug.Log("Showing Results");
        resultPanel.SetText(
            $"Test Type {currentStats.TestType} Result\n" +
            $"Enemy Damage Dealt: {currentStats.EnemyDamageDealt}\n" +
            $"Player Damage Dealt: {currentStats.PlayerDamageDealt}\n" +
            $"Game Time: {FormatTimeSpan(currentStats.GameTime)}\n" +
            $"Enemy Attack Count: {currentStats.EnemyAttackCount} \n" +
            $"Enemy Attack hit: {currentStats.EnemyAttackHit} \n" +
            $"Enemy Attack Accuracy: {currentStats.EnemyAttackAccuracy}% \n" +
            $"Did Player Win: {currentStats.isPlayerWin} \n"
        );
        resultPanel.gameObject.SetActive(true);
    }

    public string FormatTimeSpan(float seconds)
    {
        var t = TimeSpan.FromSeconds(seconds);

        return string.Format("{0:00}:{1:00}", t.Minutes, t.Seconds);
    }
}