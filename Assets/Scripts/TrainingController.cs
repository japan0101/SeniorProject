using UnityEngine;
using System.Collections.Generic;
using AutoPlayerScript;
using EnemiesScript;
using Unity.MLAgents;

public class TrainingArena : MonoBehaviour
{
    [Header("Agent Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject meleeEnemyPrefab;
    [SerializeField] private GameObject rangeEnemyPrefab;

    [Header("Spawning Configuration")]
    [SerializeField] private int numberOfMeleeEnemies = 1;
    [SerializeField] private int numberOfRangeEnemies = 1;
    [SerializeField] private float spawnAreaRadius = 10f;

    // --- MODIFIED ---
    // List for tracking LIVING enemies to check for win conditions.
    private List<EnemyAgent> activeEnemies = new List<EnemyAgent>();
    // NEW LIST: Keeps a reference to ALL agents that started the episode.
    private List<Agent> episodeParticipants = new List<Agent>();
    
    private bool matchIsOver = false;

    void Start()
    {
        ResetArena();
    }

    private void ResetArena()
    {
        matchIsOver = false;

        // --- 1. Clean up old GameObjects ---
        // We no longer need to manually destroy agents, as the parent is the arena.
        // Destroying the old participants' GameObjects is a good practice.
        foreach (var agent in episodeParticipants)
        {
            if (agent != null) Destroy(agent.gameObject);
        }
        
        // --- 2. Clear the tracking lists ---
        activeEnemies.Clear();
        episodeParticipants.Clear();

        // --- 3. Spawn New Agents ---
        SpawnPlayer();
        for (int i = 0; i < numberOfMeleeEnemies; i++)
        {
            SpawnEnemy(meleeEnemyPrefab);
        }
        for (int i = 0; i < numberOfRangeEnemies; i++)
        {
            SpawnEnemy(rangeEnemyPrefab);
        }
    }

    public void EnemyDefeated(EnemyAgent defeatedEnemy)
    {
        if (matchIsOver) return;
        // Only remove from the list of ACTIVE enemies.
        activeEnemies.Remove(defeatedEnemy);

        if (activeEnemies.Count == 0)
        {
            HandlePlayerWin();
        }
    }

    public void PlayerDefeated(AutoPlayerAgent defeatedPlayer)
    {
        if (matchIsOver) return;
        HandleEnemiesWin();
    }

    private void HandlePlayerWin()
    {
        matchIsOver = true;
        // Safely find the player reference from our participant list.
        var player = episodeParticipants.Find(p => p is AutoPlayerAgent) as AutoPlayerAgent;
        if (player != null) player.AddReward(1.0f);
        EndMatch();
    }

    private void HandleEnemiesWin()
    {
        matchIsOver = true;
        foreach (var enemy in activeEnemies) // Reward only surviving enemies
        {
            enemy.AddReward(1.0f);
        }
        var player = episodeParticipants.Find(p => p is AutoPlayerAgent) as AutoPlayerAgent;
        if (player != null) player.AddReward(-1.0f);
        EndMatch();
    }

    private void EndMatch()
    {
        // --- THE KEY CHANGE IS HERE ---
        // Iterate through our reliable list of all participants.
        // These references are valid even if their GameObjects have been destroyed.
        foreach (var agent in episodeParticipants)
        {
            if (agent != null)
            {
                agent.EndEpisode();
            }
        }
        Invoke(nameof(ResetArena), 0.5f);
    }
    
    private void SpawnPlayer()
    {
        GameObject playerObj = Instantiate(playerPrefab, GetRandomSpawnPosition(), Quaternion.identity, transform);
        AutoPlayerAgent playerAgent = playerObj.GetComponent<AutoPlayerAgent>();
        playerAgent.arenaController = this;
        
        // --- MODIFIED ---
        // Add to the list of all participants.
        episodeParticipants.Add(playerAgent);
    }
    
    private void SpawnEnemy(GameObject prefab)
    {
        GameObject enemyObj = Instantiate(prefab, GetRandomSpawnPosition(), Quaternion.identity, transform);
        EnemyAgent newEnemy = enemyObj.GetComponent<EnemyAgent>();
        newEnemy.arenaController = this;
        
        // --- MODIFIED ---
        // Add to BOTH lists.
        activeEnemies.Add(newEnemy);
        episodeParticipants.Add(newEnemy);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 randomPos = Random.insideUnitSphere * spawnAreaRadius;
        randomPos.y = 0;
        return transform.position + randomPos;
    }
}