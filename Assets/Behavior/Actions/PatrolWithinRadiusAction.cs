using EnemiesScript;
using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Random = UnityEngine.Random;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "PatrolWithinRadius", story: "Agent Patrol Within [Radius] while searching for [Target] with [Sensor]", category: "Action", id: "615bcd7faa2fd62ef86c8d2d77468f83")]
public partial class PatrolWithinRadiusAction : Composite
{
    // --- INPUTS ---
    [SerializeReference] public BlackboardVariable<float> Radius;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public BlackboardVariable<GameObject> Sensor;
    [SerializeReference] public BlackboardVariable<Terrain> terrain;

    // --- SETTINGS ---
    [SerializeReference] public BlackboardVariable<float> StoppingDistance = new BlackboardVariable<float>(1.0f);

    // --- INTERNAL STATE ---
    private Enemy enemyController;
    private Vector3 targetPosition;
    private bool hasValidTarget = false;

    // --- OOUT NODE ---
    [SerializeReference] public Node PlayerFound;
    [SerializeReference] public Node PlayerNotFound;

    private SightDetector sight;
    protected override Status OnStart()
    {
        // 1. Get the Controller
        if (GameObject == null) return Status.Failure;
        enemyController = GameObject.GetComponent<Enemy>();

        if (enemyController == null)
        {
            LogFailure("No EnemyController found!");
            return Status.Failure;
        }

        // 2. Find a destination
        targetPosition = GetRandomPointOnTerrain();

        // If we couldn't find a point (e.g., raycast failed), fail the node
        if (!hasValidTarget)
        {
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (!hasValidTarget || enemyController == null) return Status.Failure;

        // VISUALIZATION: Draw a line to where we are going
        Debug.DrawLine(GameObject.transform.position, targetPosition, Color.cyan);

        // 3. Check Distance
        float distance = Vector3.Distance(GameObject.transform.position, targetPosition);
        if (IsPlayerFound())
        {
            StopMovement();
            return StartNode(PlayerFound);
        }
        if (distance <= StoppingDistance.Value)
        {
            StopMovement();
            return StartNode(PlayerNotFound);
        }

        // 4. Move the Puppet
        MoveTowards(targetPosition);

        return Status.Running;
    }
    public bool IsPlayerFound()
    {
        // 1. Safety Checks
        if (Sensor.Value == null) return false;

        // 2. Cache the component (Optimization)
        if (sight == null || sight.gameObject != Sensor.Value)
        {
            sight = Sensor.Value.GetComponent<SightDetector>();
        }

        if (sight == null) return false;

        // 3. Ask the Perception System
        bool foundTarget = sight.IsTargetInRange;

        if (foundTarget)
        {
            // SUCCESS: We see something!
            // Write the found object into the Blackboard so other nodes (like Move/Attack) can use it
            Target.Value = sight.getTarget();
            return true;
        }

        // FAILURE: We see nothing.
        return false;
    }
    protected override void OnEnd()
    {
        if (enemyController != null)
        {
            StopMovement();
        }
    }

    // --- MOVEMENT LOGIC ---
    private void MoveTowards(Vector3 target)
    {
        Vector3 directionToTarget = target - GameObject.transform.position;
        Vector3 localDir = GameObject.transform.InverseTransformDirection(directionToTarget.normalized);
        float angleToTarget = Vector3.SignedAngle(GameObject.transform.forward, directionToTarget, Vector3.up);
        float turnAmount = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
        float forwardAmount = Mathf.Abs(angleToTarget) < 90f ? 1f : 0f;

        enemyController.RotateAgent(turnAmount);
        enemyController.MoveAgentZ(forwardAmount);
        enemyController.MoveAgentX(0f);
    }

    private void StopMovement()
    {
        enemyController.MoveAgentX(0);
        enemyController.MoveAgentZ(0);
        enemyController.RotateAgent(0);
    }

    // --- RAYCAST LOGIC ---
    public Vector3 GetRandomPointOnTerrain()
    {
        hasValidTarget = false;
        Vector3 randomPoint = Vector3.zero;
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomCircle = Random.insideUnitCircle * Radius;
            Vector3 center = GameObject.transform.position;

            float terrainHeight = terrain.Value != null ? terrain.Value.transform.position.y : 0;
            Vector3 rayStart = new Vector3(center.x + randomCircle.x, terrainHeight + 50f, center.z + randomCircle.y);

            RaycastHit hit;

            // Note: '10' in Raycast is a raw mask. If you meant Layer 10, use (1 << 10).
            // Currently, '10' means Layer 1 + Layer 3. 
            // I'll keep your '10', but verify your layer settings!
            if (Physics.Raycast(rayStart, Vector3.down, out hit, 100f, (1 << 10)))
            {
                // VISUALIZATION: Success (Green)
                Debug.DrawLine(rayStart, hit.point, Color.green, 2.0f); // Lasts 2 seconds

                randomPoint = hit.point;
                hasValidTarget = true;
                return randomPoint;
            }
            else
            {
                // VISUALIZATION: Failure (Red)
                // Draw a ray down 100 units to show where it missed
                Debug.DrawLine(rayStart, rayStart + (Vector3.down * 100f), Color.red, 2.0f);
            }
        }

        Debug.LogWarning("PatrolAction: Could not find valid terrain point.");
        return GameObject.transform.position;
    }
}