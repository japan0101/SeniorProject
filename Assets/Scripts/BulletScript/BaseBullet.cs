using System.Collections.Generic;
using AutoPlayerScript;
using UnityEngine;

public class MainBullet : MonoBehaviour
{
    [Header("Bullet data")] public float lifetime;

    public float damage;
    public AutoPlayerAgent agent;

    private readonly List<OnHitBehavior> hitBehaviors = new();

    private int groundLayer = 10;
    protected bool isMissed = true;

    private Vector3 lastVelocity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //detect and set behavior component and define life time of the bullet
        GetComponents(hitBehaviors);
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    private void Update()
    {
        //save last fram's velocity for any custom bouncing calculation
        lastVelocity = gameObject.GetComponent<Rigidbody>().linearVelocity;
    }

    private void OnDestroy()
    {
        // This method is called when the bullet GameObject is destroyed.
        // We check the flag to see if it was a hit or a miss.
        if (isMissed && agent)
            // If the bullet was destroyed and hasHit is false, it means it missed.
            TrainerManager.BulletMiss(agent);
        else
            TrainerManager.BulletHitEnemy(agent);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);
        if (other.gameObject.layer == 7) return;
        foreach (var behavior in hitBehaviors) behavior.OnBulletHit(other, lastVelocity);
        if (other.CompareTag("Enemy")) isMissed = false;
    }
}