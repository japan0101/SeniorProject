using UnityEngine;

public class NormalHit : OnHitBehavior
{
    public override void OnBulletHit(int hitLayer)
    {
        if (hitLayer == groundLayer)
        {
            Destroy(gameObject, 2);
        }
        else if (hitLayer == enemyLayer)
        {
            Destroy(gameObject);
        }
        //throw new System.NotImplementedException();
    }
}
