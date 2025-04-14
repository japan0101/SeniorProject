using UnityEngine;

public class NormalHit : OnHitBehavior
{
    public override void OnBulletHit(Collision other, Vector3 lastVelocity)
    {
        if (other.gameObject.layer == groundLayer)
        {
            Destroy(gameObject);
        }
        if (other.gameObject.layer == enemyLayer)
        {
            Destroy(gameObject);
        }
        //throw new System.NotImplementedException();
    }
}
