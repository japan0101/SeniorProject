using UnityEngine;

public abstract class OnHitBehavior : MonoBehaviour
{
    protected int groundLayer = 10;
    protected int enemyLayer = 11;
    protected int playerAtkLayer = 9;
    public abstract void OnBulletHit(Collision other, Vector3 lastVelocity);
    public abstract void OnBulletHit(Collider other, Vector3 lastVelocity);
}

