using AutoPlayerScript;
using System;
using UnityEngine;

public static class TrainerManager
{
    public static event Action<AutoPlayerAgent> OnBulletHitEnemy;

    // An event for when the bullet is destroyed without hitting an enemy (a miss).
    public static event Action<AutoPlayerAgent> OnBulletMiss;

    public static event Action<AutoPlayerAgent> OnKillEnemy;

    // A method to invoke the OnBulletHitEnemy event.
    public static void BulletHitEnemy(AutoPlayerAgent shooter)
    {
        OnBulletHitEnemy?.Invoke(shooter);
    }

    // A method to invoke the OnBulletMiss event.
    public static void BulletMiss(AutoPlayerAgent shooter)
    {
        OnBulletMiss?.Invoke(shooter);
    }
    public static void KillEnemy(AutoPlayerAgent shooter)
    {
        OnKillEnemy?.Invoke(shooter);
    }
}
