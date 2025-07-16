using UnityEngine;

public class EnemyMeleeAttack : BaseEnemyAttack
{

    private GameObject _activeSlash;
    private float _currentAngle = 0f;
    private bool _isSlashing = false;

    void Update()
    {
    }

    // Call this to start the slash (e.g., in an animation event or attack script)
    public override void OnAttack()
    {
    }
}
