using System.Collections.Generic;
using EnemiesScript.Range;
using Unity.MLAgents;
using UnityEngine;

public class TrainingController : MonoBehaviour
{
    [Header("Agent Prefabs")] [SerializeField]
    private GameObject playerPrefab;

    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangeEnemyPrefab;
    [SerializeField] private GameObject bossEnemyPrefab;

    [Header("Spawning Configuration")] [SerializeField]
    private int numberOfMeleeEnemies = 1;

    [SerializeField] private int numberOfRangeEnemies = 1;
    [SerializeField] private float spawnAreaRadius = 10f;

    [Header("Episode Settings")]
    [SerializeField] private int maxStepsPerEpisode = 5000;

    // --- MODIFIED ---
    // List for tracking LIVING enemies to check for win conditions.
    private readonly List<EnemyAgent> activeEnemies = new();

    // NEW LIST: Keeps a reference to ALL agents that started the episode.
    private readonly List<Agent> episodeParticipants = new();

    private bool matchIsOver;
    private int currentStepCount;

    private void Start()
    {
        ResetArena();
    }

    private void FixedUpdate()
    {
        if (matchIsOver) return;
        currentStepCount++;
        if (currentStepCount >= maxStepsPerEpisode)
        {
            Debug.Log("Episode timed out");
            EndMatch(); // ends for ALL agents simultaneously
        }
    }

    private void ResetArena()
    {
        matchIsOver = false;
        currentStepCount = 0; // reset shared step counter

        // --- 1. Clean up old GameObjects ---
        // We no longer need to manually destroy agents, as the parent is the arena.
        // Destroying the old participants' GameObjects is a good practice.
        foreach (var agent in episodeParticipants)
            if (agent != null)
                Destroy(agent.gameObject);

        // --- 2. Clear the tracking lists ---
        activeEnemies.Clear();
        episodeParticipants.Clear();

        // --- 3. Spawn New Agents ---
        var player = SpawnPlayer();

        // Spawn Boss and wire mutual listeners
        if (bossEnemyPrefab != null)
        {
            var boss = SpawnEnemy(bossEnemyPrefab);
            // Boss listens to player getting hurt → OnAttackLanded fires on boss
            boss.GetComponent<EnemyAgent>().agent.AddListenerToTarget(player.gameObject);
            // Player listens to boss getting hurt → OnAttackLanded fires on player
            player.GetComponent<EnemyAgent>().agent.AddListenerToTarget(boss.gameObject);
        }

        // Melee enemies (if any)
        for (var i = 0; i < numberOfMeleeEnemies; i++)
        {
            var enemy = SpawnEnemy(meleeEnemyPrefab);
            enemy.GetComponent<EnemyAgent>().agent.AddListenerToTarget(player.gameObject);
            player.GetComponent<EnemyAgent>().agent.AddListenerToTarget(enemy.gameObject);
        }
    }

    public void EnemyDefeated(EnemyAgent defeatedEnemy)
    {
        if (matchIsOver) return;
        // Only remove from the list of ACTIVE enemies.
        activeEnemies.Remove(defeatedEnemy);
        Debug.Log(activeEnemies.Count);

        if (activeEnemies.Count == 0) HandlePlayerWin();
    }

    public void PlayerDefeated(RangeEnemyAgent defeatedPlayer)
    {
        if (matchIsOver) return;
        HandleEnemiesWin();
    }

    private void HandlePlayerWin()
    {
        matchIsOver = true;
        Debug.Log("Player Won");
        // Safely find the player reference from our participant list.
        var player = episodeParticipants.Find(p => p is RangeEnemyAgent) as RangeEnemyAgent;
        if (player != null) player.AddReward(1.0f);
        EndMatch();
    }

    private void HandleEnemiesWin()
    {
        matchIsOver = true;
        Debug.Log("Enemies Won");
        foreach (var enemy in activeEnemies) // Reward only surviving enemies
            enemy.AddReward(1.0f);
        var player = episodeParticipants.Find(p => p is RangeEnemyAgent) as RangeEnemyAgent;
        if (player != null) player.AddReward(-1.0f);
        EndMatch();
    }

    private void EndMatch()
    {
        // --- THE KEY CHANGE IS HERE ---
        // Iterate through our reliable list of all participants.
        // These references are valid even if their GameObjects have been destroyed.
        foreach (var agent in episodeParticipants)
            if (agent != null)
                agent.EndEpisode();

        Invoke(nameof(ResetArena), 0.5f);
    }

    private GameObject SpawnPlayer()
    {
        var playerObj = Instantiate(playerPrefab, transform);

        playerObj.transform.localPosition = GetRandomSpawnPosition();
        playerObj.transform.localRotation = Quaternion.identity;

        var playerAgent = playerObj.GetComponent<RangeEnemyAgent>();
        playerAgent.arenaController = this;

        // --- MODIFIED ---
        // Add to the list of all participants.
        episodeParticipants.Add(playerAgent);
        return playerObj;
    }

    private GameObject SpawnEnemy(GameObject prefab)
    {
        if (prefab == null) return null;
        var enemyObj = Instantiate(prefab, transform);

        enemyObj.transform.localPosition = GetRandomSpawnPosition();
        enemyObj.transform.localRotation = Quaternion.identity;

        var newEnemy = enemyObj.GetComponent<EnemyAgent>();
        newEnemy.arenaController = this;

        // --- MODIFIED ---
        // Add to BOTH lists.
        activeEnemies.Add(newEnemy);
        episodeParticipants.Add(newEnemy);
        return enemyObj;
    }

    private Vector3 GetRandomSpawnPosition()
    {
        var x = Random.Range(-spawnAreaRadius, spawnAreaRadius);
        var z = Random.Range(-spawnAreaRadius, spawnAreaRadius);

        return new Vector3(x + 50, 5, z + 50);
    }
}