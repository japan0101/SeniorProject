using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class EnemyMeleeAttack : BaseEnemyAttack
{

    public GameObject Slash;
    GameObject atk;


    void Update()
    {
        if (atk != null)
        {
            atk.transform.RotateAround(transform.position, transform.right, 45f * Time.deltaTime);//if there are atttack hitbox available then rotate it around parent object
        }
    }

    // Call this to start the slash (e.g., in an animation event or attack script)
    public override void OnAttack()
    {
        atk = Instantiate(Slash, transform);//create attack hitbox
        atk.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);//move the hitbox to the starting point
        //Destroy(atk, 1);
    }
}
