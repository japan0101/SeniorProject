using UnityEngine;

public class ExplodingHit : OnHitBehavior
{
    [Header("BulletAttribute")]
    public float blastRadius;
    public override void OnBulletHit(Collision other, Vector3 lastVelocity)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, blastRadius);

        foreach (var hitCollider in hitColliders)
        {
            // Check if the object has a specific component if needed
            if (hitCollider.GetComponent<BaseEnemy>() != null)
            {
                hitCollider.GetComponent<BaseEnemy>().TakeDamage(hitCollider.transform.position, GetComponent<MainBullet>().damage);
                Debug.Log("Enemy detected: " + hitCollider.name);
            }
        }

        Destroy(gameObject);
    }
}
