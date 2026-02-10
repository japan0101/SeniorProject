using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using EnemiesScript;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Move Agent X", story: "Move Agent X by [amount]", category: "Enemy Actions", id: "Custom/EnemyMoveX")]
public class MoveAgentXAction : Action
{
    [SerializeReference] public BlackboardVariable<float> amount;

    // Cache the controller so we don't GetComponent every frame
    private Enemy enemyController;

    protected override Status OnStart()
    {
        // Try to find the controller on the agent that owns this graph
        if (GameObject == null) return Status.Failure;

        enemyController = GameObject.GetComponent<Enemy>();

        if (enemyController == null)
        {
            LogFailure("No EnemyController found on this GameObject.");
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (enemyController == null) return Status.Failure;

        // --- THE BRIDGE ---
        // Pass the value from the Behavior Graph to your existing EnemyController
        enemyController.MoveAgentX(amount.Value);

        // Return Running so it keeps moving this frame. 
        // Or return Success if this was a one-off command.
        return Status.Running;
    }

    protected override void OnEnd()
    {
        // Optional cleanup
    }
}