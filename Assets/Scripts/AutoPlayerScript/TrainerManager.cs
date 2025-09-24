using System;
using UnityEngine;

public static class TrainerManager
{
    public static event Action OnBulletHitEnemy;

    // An event for when the bullet is destroyed without hitting an enemy (a miss).
    public static event Action OnBulletMiss;

    // A method to invoke the OnBulletHitEnemy event.
    public static void BulletHitEnemy()
    {
        OnBulletHitEnemy?.Invoke();
    }

    // A method to invoke the OnBulletMiss event.
    public static void BulletMiss()
    {
        OnBulletMiss?.Invoke();
    }
}
