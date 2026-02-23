using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "AttackDecision", story: "Decide to attack based on [Self] and [Target]", category: "Flow", id: "dfd4461560b4e2421aff91ff066fe500")]
public partial class AttackDecisionSequence : Composite
{
    [SerializeReference] public BlackboardVariable<GameObject> Self;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public Node BasicSlash;
    [SerializeReference] public Node Thrust;
    [SerializeReference] public Node WarCry;
    [SerializeReference] public Node BodySlam;
    [SerializeReference] public Node JumpSlam;
    [SerializeReference] public Node EvadeSlash;
    [SerializeReference] public Node DontAttack;

    protected override Status OnStart()
    {

        //return StartNode(DontAttack);
        //return StartNode(BasicSlash);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {

        return Status.Success;
    }

    protected override void OnEnd()
    {

    }
    private bool IsTargetInFront(Transform target, float viewAngleThreshold)
    {
        Vector3 toTarget = (target.position - Self.Value.transform.position).normalized;
        float dotProduct = Vector3.Dot(Self.Value.transform.forward, toTarget);

        // Check if the dot product is greater than a threshold for a "vision cone"
        // A value of 0.5f corresponds roughly to a 60-degree angle (cos(60 deg) = 0.5)
        // Use a value closer to 1.0f for a narrower, more direct field of view
        if (dotProduct > viewAngleThreshold)
        {
            return true;
        }

        return false;
    }
}

