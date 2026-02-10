using EnemiesScript;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "ChaseToTarget", story: "Chase Agent Toward [Target]", category: "Action", id: "097392e53ef54f9afae2d6093a2ffe24")]
public class MoveTowardsTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<float> StoppingDistance = new BlackboardVariable<float>(1.5f);
    [SerializeReference] public BlackboardVariable<float> AlignSpeed = new BlackboardVariable<float>(0.5f); // Sensitivity

    private Enemy enemyController;

    protected override Status OnStart()
    {
        if (GameObject == null) return Status.Failure;
        enemyController = GameObject.GetComponent<Enemy>();
        return enemyController != null ? Status.Running : Status.Failure;
    }

    protected override Status OnUpdate()
    {
        if (enemyController == null || Target.Value == null) return Status.Failure;

        // 1. Calculate Vector to Target
        Vector3 directionToTarget = Target.Value.transform.position - GameObject.transform.position;
        directionToTarget.y = 0; // Flatten height so we don't look up/down

        float distance = directionToTarget.magnitude;

        // 2. Check Stopping Distance
        if (distance <= StoppingDistance.Value)
        {
            StopMovement();
            return Status.Success;
        }

        // 3. Calculate Rotation (The Turn)
        // Get the angle between where we are looking (forward) and where we want to go (directionToTarget)
        float angleToTarget = Vector3.SignedAngle(GameObject.transform.forward, directionToTarget, Vector3.up);

        // Normalize angle (-180 to 180) to a steering value (-1 to 1)
        // We clamp it so the agent turns at full speed until it's roughly facing the target
        float turnAmount = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);

        // 4. Calculate Forward Movement (The Gas)
        // Only move forward if we are mostly facing the target (angle is small)
        // This prevents "strafing" sideways while turning
        float forwardAmount = Mathf.Abs(angleToTarget) < 90f ? 1f : 0f;

        // 5. Apply to Controller
        enemyController.RotateAgent(turnAmount);
        enemyController.MoveAgentZ(forwardAmount);
        enemyController.MoveAgentX(0f); // We usually don't strafe when using tank/steering controls

        return Status.Running;
    }

    private void StopMovement()
    {
        enemyController.MoveAgentX(0);
        enemyController.MoveAgentZ(0);
        enemyController.RotateAgent(0);
    }
}
