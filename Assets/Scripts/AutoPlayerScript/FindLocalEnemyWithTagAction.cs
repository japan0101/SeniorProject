using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find Local Enemy With Tag", story: "Find Local [Object] with tag: [Tag] in [Env]", category: "Action/Find", id: "b488f99e00e383e9a6d4429689777af3")]
public partial class FindLocalEnemyWithTagAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Object;
    [SerializeReference] public BlackboardVariable<string> Tag;

    // Add a reference to the GameObject that holds this behavior graph.
    [SerializeReference] public BlackboardVariable<GameObject> Env;

    protected override Status OnStart()
    {
        if (Object == null || Env == null || string.IsNullOrEmpty(Tag.Value))
        {
            // Return failure if any necessary variables are not set.
            return Status.Failure;
        }

        // Iterate through all child transforms of the Self object's parent.
        // This finds a sibling with the specified tag.
        Transform parent = Env.Value.transform;
        if (parent != null)
        {
            foreach (Transform child in parent)
            {
                if (child.CompareTag(Tag.Value))
                {
                    Object.Value = child.gameObject;
                    return Status.Success;
                }
            }
        }

        // If the parent is null (top-level GameObject) or no object is found, try finding a child instead.
        foreach (Transform child in Env.Value.transform)
        {
            if (child.CompareTag(Tag.Value))
            {
                Object.Value = child.gameObject;
                return Status.Success;
            }
        }

        return Status.Failure;
    }
}


