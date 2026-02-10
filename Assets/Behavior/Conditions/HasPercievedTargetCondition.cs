using System;
using Unity.Behavior;
using UnityEngine;

[Serializable, Unity.Properties.GeneratePropertyBag]
[Condition(name: "HasPercievedTarget", story: "Check if [Sensor] sees a [Target]", category: "Conditions", id: "8656a65120da8c8ee9aa932ec9100030")]
public partial class HasPercievedTargetCondition : Condition
{
    // INPUT: The GameObject that has the PerceptionSystem script
    [SerializeReference] public BlackboardVariable<GameObject> Sensor;

    // OUTPUT: Where we will store the target if we find one
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    private SightDetector sight;

    public override bool IsTrue()
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

    public override void OnStart()
    {
    }

    public override void OnEnd()
    {
    }
}
