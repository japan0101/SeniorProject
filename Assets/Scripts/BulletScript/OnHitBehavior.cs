using UnityEngine;

public abstract class OnHitBehavior : MonoBehaviour
{
    protected int groundLayer = 10;
    protected int enemyLayer = 11;
    protected int playerAtkLayer = 9;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public abstract void OnBulletHit(int hitLayer);
}

