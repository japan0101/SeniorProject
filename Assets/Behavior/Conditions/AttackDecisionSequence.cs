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
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

