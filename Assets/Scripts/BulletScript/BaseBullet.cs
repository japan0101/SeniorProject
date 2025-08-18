using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using static UnityEngine.InputSystem.OnScreen.OnScreenStick;

public class MainBullet : MonoBehaviour
{
    [Header("Bullet data")]
    public float lifetime;
    public float damage;

    private List<OnHitBehavior> hitBehaviors = new List<OnHitBehavior>();

    private int groundLayer = 10;
    private Vector3 lastVelocity;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //detect and set behavior component and define life time of the bullet
        GetComponents<OnHitBehavior>(hitBehaviors);
        Destroy(gameObject, lifetime);
    }
    private void OnTriggerEnter(Collider other)
    {
        foreach (var behavior in hitBehaviors)
        {
            behavior.OnBulletHit(other, lastVelocity);
        }
    }
    //private void OnCollisionEnter(Collision collision)
    //{
    //    //execute any function that should happend on collision
    //    foreach (var behavior in hitBehaviors)
    //    {
    //        behavior.OnBulletHit(collision, lastVelocity);
    //    }
    //}
    // Update is called once per frame
    void Update()
    {
        //save last fram's velocity for any custom bouncing calculation
        lastVelocity = gameObject.GetComponent<Rigidbody>().linearVelocity;
    }
}