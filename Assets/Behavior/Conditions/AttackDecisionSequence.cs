using System;
using Unity.Behavior;
using UnityEngine;
using Composite = Unity.Behavior.Composite;
using Unity.Properties;
using EnemiesScript;

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
        if (Self.Value == null || Target.Value == null) return Status.Failure;

        // 1. Calculate Spatial Data upfront
        Vector3 vectorToTarget = Target.Value.transform.position - Self.Value.transform.position;
        float distance = vectorToTarget.magnitude;
        Vector3 direction = vectorToTarget.normalized;
        float dotProduct = Vector3.Dot(Self.Value.transform.forward, direction);

        // 2. Define Vision Cones
        // 0.5f is a standard cone (~60 degrees).
        bool isTargetDeadAhead = dotProduct > 0.80f;
        bool isTargetInFront = dotProduct > 0.5f;

        // 3. Cache Enemy Component Data
        Enemy enemyComponent = Self.Value.GetComponent<Enemy>();
        float currentHp = enemyComponent != null ? enemyComponent.hp : float.MaxValue;

        // --- OUT OF RANGE ---
        if (distance >= 20f)
        {
            Debug.Log("Target >20 range. Don't Attack.");
            return StartNode(DontAttack);
        }

        // --- LONG RANGE (15 to 20) ---
        if (distance >= 15f)
        {
            Debug.Log("15-20 range. Use Body Slam *Add forward velocity");
            return StartNode(BodySlam);
        }

        // --- NARROW CONE OVERRIDE (Applies to all ranges < 15) ---
        if (isTargetDeadAhead)
        {
            Debug.Log("Target dead ahead with <15 range. Use Thrust.");
            return StartNode(Thrust);
        }

        // --- MEDIUM RANGE (10 to 15) ---
        if (distance >= 10f)
        {
            if (isTargetInFront)
            {
                Debug.Log("10-15 range, in front. Use Jump Slam *Add forward velocity");
                return StartNode(JumpSlam);
            }
            else
            {
                Debug.Log("10-15 range, not in front. Use War Cry.");
                return StartNode(WarCry);
            }
        }

        // --- CLOSE RANGE (< 10) ---
        if (isTargetInFront)
        {
            if (currentHp < 200f && distance <= 7f)
            {
                Debug.Log("<10 range, low health, <=7 dist. Use Evade Slash.");
                return StartNode(EvadeSlash);
            }
            else
            {
                Debug.Log("<10 range, in front. Use Basic Slash.");
                return StartNode(BasicSlash);
            }
        }

        // Fallback for close range but target is behind the enemy
        Debug.Log("<10 range, but target is behind. Don't Attack.");
        return StartNode(DontAttack);
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}