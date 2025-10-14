using EnemiesScript;
using Unity.MLAgents;
using UnityEngine;

public abstract class EnemyAgent : Agent
{
    [SerializeField] public Enemy agent;
    [HideInInspector] public int currentEpisode;
    [HideInInspector] public float cumulativeReward;
    public bool isTraining;
    protected bool _attackedThisStep = false;
    protected float _lastDistance;
    protected float _minAttackDistance = 1.5f;
    public TrainingController arenaController;
    public abstract void OnAttack();
    public abstract void OnSpecial();
    public abstract void OnAttackMissed();//Called by Enemy attack event listener to notify that the attack launched did not and on a player
    public abstract void OnAttackLanded();// Called when Agent Hit Something
    public abstract void OnKilledTarget();// Called when Agent Kill Something
    public abstract void OnKilled();
    public abstract void OnHurt();
}
