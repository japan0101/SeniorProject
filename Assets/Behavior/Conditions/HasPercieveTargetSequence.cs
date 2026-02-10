using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "HasPercieveTarget", story: "Check if [Sensor] sees a [Target]", category: "Flow", id: "e688d0a3b587105bfea5b3341a61de3d")]
public partial class HasPercieveTargetSequence : Composite
{
    [SerializeReference] public BlackboardVariable<GameObject> Sensor;
    [SerializeReference] public BlackboardVariable<GameObject> Target;
    [SerializeReference] public Node True;
    [SerializeReference] public Node False;

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

