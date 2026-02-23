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

        //return StartNode(DontAttack);
        //return StartNode(BasicSlash);
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if( Self == null || Target == null) return Status.Failure;
        Vector3 _vectorToTarget = Self.Value.transform.position - Target.Value.transform.position;
        float _distanceToTarget = _vectorToTarget.magnitude;
        if (_distanceToTarget < 20)
        {
            if (_distanceToTarget < 15)
            {
                if (IsTargetInFront(Target.Value.transform, 0.1f))
                {

                    //thrust at 0.0-0.1 with <15 range
                    StartNode(Thrust);
                }
                else if (_distanceToTarget < 10)
                {
                    if (IsTargetInFront(Target.Value.transform, 0.5f))
                    {
                        if(Self.Value.GetComponent<Enemy>().hp < 200 && _distanceToTarget <= 7)
                        {
                            //If <10 range and low health up front use evade slash
                            StartNode(EvadeSlash);
                        }
                        else
                        {
                            //Basic Slash at 0.1-0.5 Viewing Angle and <10 range
                            StartNode(BasicSlash);
                        }
                    }
                    else
                    {
                        return StartNode(DontAttack);
                    }
                }
                else if (IsTargetInFront(Target.Value.transform, 0.5f))
                {
                    //<15 range but in front will use jump slam *need to add forward velocity
                    StartNode(JumpSlam);
                }
                else
                {
                    //<15 range will use war cry
                    StartNode(WarCry);
                }
            }
            else
            {
                //<20 range use Body Slam *Add forward velocity too
                StartNode(BodySlam);
            }
        }
        else
        {
            return StartNode(DontAttack);
        }
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

