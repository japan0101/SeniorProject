using UnityEngine;

public class EnemyMeleeAttack : BaseEnemyAttack
{
    [Header("Slash Settings")]
    [SerializeField] private GameObject _slashObject; // Prefab to spawn (trail/hitbox)
    [SerializeField] private float _slashRadius = 2f; // Distance from enemy
    [SerializeField] private float _slashSpeed = 1f; // Rotation speed
    [SerializeField] private float _arcAngle = 180f; // Semi-circle (180 degrees)
    [SerializeField] private bool _clockwise = true; // Direction of slash

    private GameObject _activeSlash;
    private float _currentAngle = 0f;
    private bool _isSlashing = false;

    void Update()
    {
        if (_isSlashing)
        {
            // Calculate movement direction (clockwise/counter-clockwise)
            float direction = _clockwise ? -1f : 1f;
            _currentAngle += _slashSpeed * Time.deltaTime * direction;

            // Convert angle to position in a semi-circle
            float radians = _currentAngle * Mathf.Deg2Rad;
            Vector3 offset = new Vector3(
                Mathf.Sin(radians) * _slashRadius,
                0f,
                Mathf.Cos(radians) * _slashRadius
            );

            // Position the slash object relative to the enemy
            _activeSlash.transform.position = transform.position + offset;
            _activeSlash.transform.rotation = Quaternion.LookRotation(offset);

            // End slash after completing the arc
            if (Mathf.Abs(_currentAngle) >= _arcAngle / 2f)
            {
                EndSlash();
            }
        }
    }

    // Call this to start the slash (e.g., in an animation event or attack script)
    public override void OnAttack()
    {
        if (_slashObject == null) return;

        _currentAngle = -_arcAngle / 2f; // Start at one end of the arc
        _activeSlash = Instantiate(_slashObject, transform.position, Quaternion.identity);
        _isSlashing = true;
    }

    private void EndSlash()
    {
        _isSlashing = false;
        if (_activeSlash != null)
        {
            Destroy(_activeSlash, 0.1f); // Optional delay for effects to fade
        }
    }
}
