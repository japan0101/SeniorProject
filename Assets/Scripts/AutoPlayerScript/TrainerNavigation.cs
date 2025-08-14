using Unity.MLAgents;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class TrainerNavigation : MonoBehaviour
{
    [Header("Behavior Settings")]
    [Tooltip("The parent GameObject containing the enemies.")]
    public GameObject environment;
    [Tooltip("Distance at which the trainer will start to flee from the target.")]
    public float distanceThreshold = 5f;
    [Tooltip("Orientation to aim the gun")]
    public GameObject aimOrientation;

    [Header("Movement Settings")]
    [Tooltip("The movement speed of the trainer.")]
    public float speed = 10f;
    [Tooltip("The speed at which the trainer can turn.")]
    public float turnSpeed = 5f;
    [Tooltip("The maximum angle (in degrees) to deviate from a perfect aim.")]
    public float aimErrorMargin = 2f;

    // A reference to the current target GameObject.
    private GameObject EnemyTarget;
    // A reference to the Rigidbody component for force-based movement.
    private Rigidbody rb;
    // The desired rotation for the agent.
    private Quaternion desiredRotation;

    void Start()
    {
        // Get the Rigidbody component once at the start.
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Find a target if one doesn't exist.
        if (EnemyTarget == null)
        {
            FindTarget("MeleeEnemy");
        }

        // Only proceed if a target has been found.
        if (EnemyTarget != null)
        {
            // Determine the direction to move and rotate.
            Vector3 moveDirection;
            Vector3 aimDirection;

            float distanceToEnemy = Vector3.Distance(transform.position, EnemyTarget.transform.position);

            if (distanceToEnemy < distanceThreshold)
            {
                // Flee: Move away from the enemy.
                moveDirection = transform.position - EnemyTarget.transform.position;
            }
            else
            {
                // Pursue: Move towards the enemy.
                moveDirection = EnemyTarget.transform.position - transform.position;
            }

            // Set the aim direction to always face the enemy, regardless of movement.
            aimDirection = EnemyTarget.transform.position - transform.position;

            // Call the movement and aiming methods.
            MoveDirection(moveDirection);
            AutoAim(aimDirection);
        }
        else
        {
            Debug.Log("Enemy not found");
        }

        // Control the agent's speed to prevent it from accelerating infinitely.
        SpeedControl();
    }

    /// <summary>
    /// Applies force to the Rigidbody to move the agent.
    /// </summary>
    /// <param name="dir">The direction to move.</param>
    private void MoveDirection(Vector3 dir)
    {
        Vector3 normalizedDirection = dir.normalized;
        rb.AddForce(normalizedDirection * speed * 10, ForceMode.Force);
    }

    /// <summary>
    /// Smoothly rotates the agent to face a target direction with a random error margin.
    /// </summary>
    /// <param name="direction">The base direction to aim towards.</param>
    private void AutoAim(Vector3 direction)
    {
        // Keep the rotation horizontal.
        Vector3 directionToAim = new Vector3(direction.x, 0, direction.z);
        Vector3 vOrientation = new Vector3(0, direction.y, 0);

        // Add a random angle deviation.
        float randomAngle = Random.Range(-aimErrorMargin, aimErrorMargin);
        Quaternion randomRotation = Quaternion.Euler(0, randomAngle, 0);

        // Apply the random rotation to the direction vector.
        Vector3 erroredDirection = randomRotation * directionToAim;

        // Calculate the target rotation.
        if (erroredDirection != Vector3.zero)
        {
            desiredRotation = Quaternion.LookRotation(erroredDirection);
        }

        // Smoothly rotate the agent.
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
        // Aim at enemy
        //aimOrientation.transform.rotation = Quaternion.Euler(vOrientation.y, 0, 0);
    }

    /// <summary>
    /// Finds the enemy target based on its tag, either globally or within a specific environment.
    /// </summary>
    /// <param name="tags">The tag of the target to find.</param>
    private void FindTarget(string tags)
    {
        // If an environment is set, search within its children.
        if (environment != null)
        {
            foreach (Transform child in environment.transform)
            {
                if (child.CompareTag(tags))
                {
                    EnemyTarget = child.gameObject;
                    return; // Exit the function once the first target is found.
                }
            }
        }
        else // Otherwise, perform a global search.
        {
            EnemyTarget = GameObject.FindGameObjectWithTag(tags);
        }
    }

    /// <summary>
    /// Limits the linear velocity of the Rigidbody to prevent over-acceleration.
    /// </summary>
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, rb.linearVelocity.z);
        }
    }
}
