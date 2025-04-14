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
        GetComponents<OnHitBehavior>(hitBehaviors);
        Destroy(gameObject, lifetime);
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    foreach (var behavior in hitBehaviors)
    //    {
    //        behavior.OnBulletHit(other, lastVelocity);
    //    }
    //}
    private void OnCollisionEnter(Collision collision)
    {
        foreach (var behavior in hitBehaviors)
        {
            behavior.OnBulletHit(collision, lastVelocity);
        }
    }
    // Update is called once per frame
    void Update()
    {
        lastVelocity = gameObject.GetComponent<Rigidbody>().linearVelocity;
    }
}