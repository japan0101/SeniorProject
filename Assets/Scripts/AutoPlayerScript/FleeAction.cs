using System;
using Unity.Behavior;
using Unity.MLAgents;
using Unity.Properties;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI; // Required for NavMeshAgent
using Action = Unity.Behavior.Action;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Flee", story: "Navigate [Self] Away From [Object]", category: "Action/Navigation", id: "a82023d4e9e666090e95b5c5a3eb432c")]
public partial class FleeAction : Action
{
    // The GameObject that will be fleeing.
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    // The GameObject that the agent will flee from.
    [SerializeReference] public BlackboardVariable<GameObject> Object;
    // The desired distance to maintain from the target.
    [SerializeReference] public BlackboardVariable<float> FleeDistance;

    // A reference to the NavMeshAgent component on the Self GameObject.
    private NavMeshAgent agent;

    protected override Status OnStart()
    {
        // Get the NavMeshAgent component from the Self GameObject.
        if (Self.Value != null)
        {
            agent = Self.Value.GetComponent<NavMeshAgent>();
        }

        // If the agent or target are missing, the action cannot succeed.
        if (agent == null || Object.Value == null)
        {
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // Ensure the agent and target still exist.
        if (agent == null || Object.Value == null)
        {
            return Status.Failure;
        }

        // Calculate the direction vector from the target to the agent.
        // This vector points away from the target.
        Vector3 fleeDirection = Self.Value.transform.position - Object.Value.transform.position;

        // Normalize the direction vector and multiply by the desired flee distance
        // to get a new position far away from the target.
        Vector3 newDestination = Self.Value.transform.position + fleeDirection.normalized * FleeDistance.Value;

        // Set the new destination for the NavMeshAgent.
        agent.SetDestination(newDestination);

        // The action is still running and will continue to update the destination
        // as long as it's active.
        return Status.Running;
    }

    protected override void OnEnd()
    {
        // Optional: Stop the agent from moving when the action ends.
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
        }
    }
}