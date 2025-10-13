using AutoPlayerScript;
using EnemiesScript;
using System.Collections;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;
public abstract class EnemyAgent : Agent
{
    [SerializeField] protected GameObject targetPrefab;
    [SerializeField] protected Renderer groundRenderer;
    [SerializeField] protected Enemy agent;
    [HideInInspector] public int currentEpisode;
    [HideInInspector] public float cumulativeReward;
    protected Transform _arena;
    public bool isTraining;
    protected Color _defaultGroundColor;
    protected Coroutine _flashGroundCoroutine;
    protected PlayerHealth _playerHealthManager;
    protected bool _attackedThisStep = false;
    protected float _lastDistance;
    protected float _minAttackDistance = 1.5f;
    public TrainingArena arenaController;
    public abstract void OnAttack();

    public abstract void OnSpecial();
    public abstract void OnAttackMissed();//Called by Enemy attack event listener to notify that the attack launched did not and on a player
    public abstract void OnAttackLanded();// Called when Agent Hit Something
    public abstract void OnKilledTarget();// Called when Agent Kill Something
    public abstract void OnKilled();
    public abstract void OnHurt();
}
