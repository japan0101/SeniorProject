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
    public float baseSpeed = 10f;
    [Tooltip("The rotate speed of the trainer.")]
    public float rotateSpeed = 10f;
    [Tooltip("The speed at which the trainer can turn.")]
    public float turnSpeed = 360;
    [Tooltip("The speed at which the trainer can turn.")]
    public float dashSpeed = 8f;
    [Tooltip("The maximum angle (in degrees) to deviate from a perfect aim.")]
    public float aimErrorMargin = 2f;

    // A reference to the current target GameObject.
    [HideInInspector]public GameObject EnemyTarget;
    // A reference to the Rigidbody component for force-based movement.
    private Rigidbody rb;
    // The desired rotation for the agent.
    private Quaternion desiredRotation;
    protected float realSpeed;
    private bool isDashing = false;

    void Start()
    {
        // Get the Rigidbody component once at the start.
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Find a target if one doesn't exist.
        // Only proceed if a target has been found.
        //if (EnemyTarget == null)
        //{
        //    //FindTarget("MeleeEnemy");
        //}
        //if (EnemyTarget != null)
        //{
        //    // Determine the direction to move and rotate.
        //    Vector3 moveDirection;
        //    Vector3 aimDirection;

        //    float distanceToEnemy = Vector3.Distance(transform.position, EnemyTarget.transform.position);

        //    if (distanceToEnemy < distanceThreshold)
        //    {
        //        // Flee: Move away from the enemy.
        //        moveDirection = transform.position - EnemyTarget.transform.position;
        //    }
        //    else
        //    {
        //        // Pursue: Move towards the enemy.
        //        moveDirection = EnemyTarget.transform.position - transform.position;
        //    }

        //    // Set the aim direction to always face the enemy, regardless of movement.
        //    aimDirection = EnemyTarget.transform.position - transform.position;

        //    // Call the movement and aiming methods.
        //    //MoveDirection(moveDirection);
        //    AutoAim(aimDirection);
        //}
        //else
        //{
        //    Debug.Log("Enemy not found");
        //}

        // Control the agent's speed to prevent it from accelerating infinitely.
        SpeedControl();
    }

    /// <summary>
    /// Applies force to the Rigidbody to move the agent.
    /// </summary>
    /// <param name="dir">The direction to move.</param>

    public void MoveAgentX(float actionValue)
    {
        SpeedControl();
        rb.AddForce(transform.right * realSpeed * actionValue * baseSpeed, ForceMode.Force);
    }

    public void MoveAgentZ(float actionValue)
    {
        SpeedControl();
        rb.AddForce(transform.forward * realSpeed * actionValue * baseSpeed, ForceMode.Force);
    }

    public void RotateAgent(float actionValue)
    {
        transform.Rotate(0f, rotateSpeed * actionValue * Time.deltaTime, 0f);
    }

    /// <summary>
    /// Smoothly rotates the agent to face a target direction with a random error margin.
    /// </summary>
    /// <param name="direction">The base direction to aim towards.</param>
    //private void AutoAim(Vector3 direction)
    //{
    //    // Keep the rotation horizontal.
    //    Vector3 directionToAim = new Vector3(direction.x, 0, direction.z);
    //    Vector3 vOrientation = new Vector3(0, direction.y, 0);

    //    // Add a random angle deviation.
    //    float randomAngle = Random.Range(-aimErrorMargin, aimErrorMargin);
    //    Quaternion randomRotation = Quaternion.Euler(0, randomAngle, 0);

    //    // Apply the random rotation to the direction vector.
    //    Vector3 erroredDirection = randomRotation * directionToAim;

    //    // Calculate the target rotation.
    //    if (erroredDirection != Vector3.zero)
    //    {
    //        desiredRotation = Quaternion.LookRotation(erroredDirection);
    //    }

    //    // Smoothly rotate the agent.
    //    transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
    //    // Aim at enemy
    //    //aimOrientation.transform.rotation = Quaternion.Euler(vOrientation.y, 0, 0);
    //}


    /// <summary>
    /// Limits the linear velocity of the Rigidbody to prevent over-acceleration.
    /// </summary>
    protected void SpeedControl()
    {
        rb.linearDamping = 2.5f;
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (isDashing)
        {
            realSpeed = dashSpeed;
        }
        else
        {
            realSpeed = baseSpeed;
        }
        if (flatVel.magnitude > realSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * baseSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
        //Debug.Log(realSpeed);
    }
}
