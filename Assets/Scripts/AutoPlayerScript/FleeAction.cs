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
    // The GameObject that will be fleeing. This object must have a Rigidbody component.
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    // The GameObject that the agent will flee from.
    [SerializeReference] public BlackboardVariable<GameObject> Object;
    // The force magnitude to apply when fleeing.
    [SerializeReference] public BlackboardVariable<float> FleeForceMagnitude;

    // A reference to the Rigidbody component on the Self GameObject.
    private Rigidbody rb;

    protected override Status OnStart()
    {
        // Get the Rigidbody component from the Self GameObject.
        if (Self.Value != null)
        {
            rb = Self.Value.GetComponent<Rigidbody>();
        }

        // If the Rigidbody or target are missing, the action cannot succeed.
        if (rb == null || Object.Value == null)
        {
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        // Ensure the Rigidbody and target still exist.
        if (rb == null || Object.Value == null)
        {
            return Status.Failure;
        }

        // Calculate the direction vector from the target to the agent.
        // This vector points away from the target.
        Vector3 fleeDirection = Self.Value.transform.position - Object.Value.transform.position;

        // Normalize the direction vector to get a unit vector.
        Vector3 normalizedFleeDirection = fleeDirection.normalized;

        // Apply a force to the Rigidbody in the flee direction.
        // We use ForceMode.Force to apply a continuous force over time.
        Debug.Log("Run away" + normalizedFleeDirection);
        rb.AddForce(normalizedFleeDirection * FleeForceMagnitude.Value, ForceMode.Force);

        // The action is still running and will continue to apply force.
        return Status.Success;
    }

    protected override void OnEnd()
    {
        // Optional: Stop the Rigidbody's movement when the action ends.
        if (rb != null)
        {
            //rb.linearVelocity = Vector3.zero;
        }
    }
}