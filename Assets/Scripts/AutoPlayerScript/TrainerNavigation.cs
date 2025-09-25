using UnityEngine;
using UnityEngine.Serialization;

namespace AutoPlayerScript
{
    public class TrainerNavigation : MonoBehaviour
    {
        [Header("Behavior Settings")] [Tooltip("The parent GameObject containing the enemies.")]
        public GameObject environment;

        [Tooltip("Distance at which the trainer will start to flee from the target.")]
        public float distanceThreshold = 5f;

        [Tooltip("Orientation to aim the gun")]
        public GameObject aimOrientation;

        [Header("Movement Settings")] [Tooltip("The movement speed of the trainer.")]
        public float baseSpeed = 8f;

        [Tooltip("The rotate speed of the trainer.")]
        public float rotateSpeed = 360f;

        [Tooltip("The speed of agent dashing")]
        public float dashSpeed = 10f;

        [Tooltip("The maximum angle (in degrees) to deviate from a perfect aim.")]
        public float aimErrorMargin = 2f;

        // A reference to the current target GameObject.
        [FormerlySerializedAs("EnemyTarget")] [HideInInspector] public GameObject enemyTarget;

        // A reference to the Rigidbody component for force-based movement.
        private Rigidbody _rb;

        // The desired rotation for the agent.
        private Quaternion _desiredRotation;
        private float _realSpeed;
        private bool _isDashing = false;


        void Awake()
        {
            // Get the Rigidbody component once at the start.
            _rb = GetComponent<Rigidbody>();
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
        /// <param name="actionValue"></param>
        public void MoveAgentX(float actionValue)
        {
            SpeedControl();
            _rb.AddForce(transform.right * _realSpeed * actionValue * baseSpeed, ForceMode.Force);
        }

        public void MoveAgentZ(float actionValue)
        {
            SpeedControl();
            _rb.AddForce(transform.forward * _realSpeed * actionValue * baseSpeed, ForceMode.Force);
        }

        public void RotateAgent(float actionValue)
        {
            transform.Rotate(0f, rotateSpeed * actionValue * Time.deltaTime, 0f);
        }


        /// <summary>
        /// Limits the linear velocity of the Rigidbody to prevent over-acceleration.
        /// </summary>
        private void SpeedControl()
        {
            _rb.linearDamping = 2.5f;
            Vector3 flatVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
            _realSpeed = _isDashing ? dashSpeed : baseSpeed;

            if (flatVel.magnitude > _realSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * baseSpeed;
                _rb.linearVelocity = new Vector3(limitedVel.x, _rb.linearVelocity.y, limitedVel.z);
            }
            //Debug.Log(realSpeed);
        }
    }
}
