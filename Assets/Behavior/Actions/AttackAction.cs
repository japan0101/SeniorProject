using EnemiesScript;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Attack", story: "Attack with [index]", category: "Action", id: "61eb8bf45b67990bfd4ac2cfd298b1d7")]
public partial class AttackAction : Action
{
    [SerializeReference] public BlackboardVariable<int> Index;
    private Enemy enemyController;
    protected override Status OnStart()
    {
        if (GameObject == null) return Status.Failure;
        enemyController = GameObject.GetComponent<Enemy>();
        return enemyController != null ? Status.Running : Status.Failure;
    }

    protected override Status OnUpdate()
    {
        Debug.Log("Attacking from bg");
        enemyController.Attack(Index);
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

