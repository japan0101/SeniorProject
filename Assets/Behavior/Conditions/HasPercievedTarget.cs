using System;
using Unity.Behavior;
using Unity.Properties;
using UnityEngine;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Has Perceived Target", story: "Check if [Sensor] sees a [Target]", category: "Perception", id: "Custom/HasPerceivedTarget")]
public class HasPerceivedTargetCondition : Condition
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
}