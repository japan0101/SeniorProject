using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Shoot", story: "Run Shoot function [in]", category: "Action", id: "e01573565c0fab0e33e1eca8897c9d7f")]
public partial class ShootAction : Action
{
    [SerializeReference] public BlackboardVariable<AutoShoot> In;

    protected override Status OnStart()
    {
        AutoShoot shootMng = In;
        if (shootMng != null) {
            shootMng.ShootWeapon();
            Console.WriteLine("pew");
            return Status.Success;
        }
        Console.WriteLine("Fucked");
        return Status.Failure;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

