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
            atk.transform.RotateAround(transform.position, transform.up, 1440f * Time.deltaTime);//if there are atttack hitbox available then rotate it around parent object
        }
    }

    // Call this to start the slash (e.g., in an animation event or attack script)
    public override void OnAttack()
    {
        atk = Instantiate(Slash, transform.position, transform.rotation);//create attack hitbox
        Destroy(atk, 0.125f);//desqpwn after a certain time
    }
}
