using AutoPlayerScript;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
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
    protected bool isMissed = true;
    public AutoPlayerAgent agent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //detect and set behavior component and define life time of the bullet
        GetComponents<OnHitBehavior>(hitBehaviors);
        Destroy(gameObject, lifetime);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.gameObject.layer);
        if(other.gameObject.layer == 7) return;
        foreach (var behavior in hitBehaviors)
        {
            behavior.OnBulletHit(other, lastVelocity);
        }
        if (other.CompareTag("Enemy"))
        {
            isMissed = false;
        }
    }
    private void OnDestroy()
    {
        // This method is called when the bullet GameObject is destroyed.
        // We check the flag to see if it was a hit or a miss.
        if (isMissed && agent)
        {
            // If the bullet was destroyed and hasHit is false, it means it missed.
            TrainerManager.BulletMiss(agent);
        }
        else
        {
            TrainerManager.BulletHitEnemy(agent);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //save last fram's velocity for any custom bouncing calculation
        lastVelocity = gameObject.GetComponent<Rigidbody>().linearVelocity;
    }
}