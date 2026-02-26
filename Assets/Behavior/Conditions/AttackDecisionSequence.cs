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

    [SerializeReference] public BlackboardVariable<float> MaxDistance;

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
        bool isTargetDeadAhead = dotProduct > 0.9f;
        bool isTargetInFront = dotProduct > 0.5f;

        //Debug.Log($"{dotProduct} is DeadAhead:{isTargetDeadAhead} or InFront:{isTargetInFront}");

        // 3. Cache Enemy Component Data
        Enemy enemyComponent = Self.Value.GetComponent<Enemy>();
        float currentHp = enemyComponent != null ? enemyComponent.hp : float.MaxValue;

        // --- OUT OF RANGE ---
        if (distance >= 20f)
        {
            Debug.Log("Target >20 range. Don't Attack.");
            MaxDistance.Value = 20f;
            return StartNode(DontAttack);
        }

        // --- LONG RANGE (15 to 20) ---

        Debug.Log($"{distance}>15:{distance >= 15f} HP<400:{currentHp < 400f} InCooldown:{enemyComponent.CanUseAttack(4)}");
        if (distance >= 15f)
        {
            if (currentHp < 400f && enemyComponent.CanUseAttack(4))
            {
                Debug.Log("15-20 range. Use Body Slam *Add forward velocity");
                return StartNode(BodySlam);
            }
            Debug.LogWarning("Fail to met any condition for long range. Don't Attack.");
            MaxDistance.Value = 15f;
            return StartNode(DontAttack);
        }

        // --- NARROW CONE OVERRIDE (Applies to all ranges < 15) ---
        if (distance < 15f)
        {
            if (isTargetDeadAhead && enemyComponent.CanUseAttack(2))
            {
                Debug.Log("Target dead ahead with <15 range. Use Thrust.");
                return StartNode(Thrust);
            }
        }

        // --- MEDIUM RANGE (10 to 15) ---
        if (distance >= 10f)
        {
            if (currentHp < 200f && isTargetInFront && enemyComponent.CanUseAttack(5))
            {
                Debug.Log("10-15 range, in front. Use Jump Slam *Add forward velocity");
                return StartNode(JumpSlam);
            }
            if (currentHp < 400f && enemyComponent.CanUseAttack(3))
            {
                Debug.Log("10-15 range, not in front. Use War Cry.");
                return StartNode(WarCry);
            }
            MaxDistance.Value = 10f;
            Debug.LogWarning("Fail to met any condition in Med range. Don't Attack.");
            return StartNode(DontAttack);
        }

        // --- CLOSE RANGE (< 10) ---
        if (isTargetInFront && distance <= 10f)
        {
            if (currentHp < 200f && enemyComponent.CanUseAttack(6))
            {
                Debug.Log("<10 range, low health, <=7 dist. Use Evade Slash.");
                return StartNode(EvadeSlash);
            }
            if (enemyComponent.CanUseAttack(1))
            {
                Debug.Log("<10 range, in front. Use Basic Slash.");
                return StartNode(BasicSlash);
            }
            MaxDistance.Value = 5f;
            Debug.LogWarning("Fail to met any condition in close range. Don't Attack.");
            return StartNode(DontAttack);
        }

        // Fallback for close range but target is behind the enemy
        MaxDistance.Value = 5f;
        Debug.LogWarning("<10 range, but target is behind. Don't Attack.");
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