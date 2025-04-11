using UnityEngine;

public class NormalHit : OnHitBehavior
{
    public override void OnBulletHit(Collider other, Vector3 lastVelocity)
    {
        if (other.gameObject.layer == groundLayer)//Hitting ground/walls will bounces
        {
            Vector3 bounceDirection = Vector3.Reflect(lastVelocity.normalized, other.ClosestPoint(transform.position) - transform.position);
            gameObject.GetComponent<Rigidbody>().linearVelocity = bounceDirection * 5;
            //gameObject.GetComponent<Collider>().isTrigger = false;
            Destroy(gameObject, 2);
        }
        if (other.gameObject.layer == enemyLayer)
        {
            Destroy(gameObject);
        }
        //throw new System.NotImplementedException();
    }
}
