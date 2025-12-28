using System.Collections;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private PlayerStats _stats;
    [Header("Movement")]

    private float speed;

    public float groundDrag;
    bool readyToDash;

    public float airMultiplier;
    bool readyToJump;

    [Header("References")]
    public EnergyManager energyManager;
    public Zoom zoomMnger;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode dashKey = KeyCode.LeftControl;

    [Header("GroundCheck")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform hOrientation;
    public Transform vOrientation;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    private Coroutine resetJumpRoutine;
    private Coroutine resetDashRoutine;

    Rigidbody rb;

    private void Start()
    {
        //set up regid body
        rb= GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        ResetJump();
        ResetDash();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded && energyManager.consumeEnergy(_stats.jumpCost))
        {
            readyToJump = false;
            Jump();

            // If there's an existing coroutine, stop it first
            if (resetJumpRoutine != null)
            {
                StopCoroutine(resetJumpRoutine);
            }

            resetJumpRoutine = StartCoroutine(ResetJumpAfterDelay(_stats.jumpCooldown));
        }

        //when dash
        if (Input.GetKey(dashKey) && readyToDash && energyManager.consumeEnergy(_stats.dashCost))
        {
            readyToDash = false;
            Dash();

            Invoke(nameof(ResetDashSpeed), _stats.dashDuration);

            // If there's an existing coroutine, stop it first
            if (resetDashRoutine != null)
            {
                StopCoroutine(resetDashRoutine);
            }

            resetDashRoutine = StartCoroutine(ResetDashAfterDelay(_stats.dashCooldown));
        }
        if (Input.GetKey(sprintKey)){
            if (!_stats.States.Contains(PlayerStates.Sprinting))
            {
                _stats.States.Add(PlayerStates.Sprinting);
            }
        }else
        {
            _stats.States.Remove(PlayerStates.Sprinting);
        }
    }

    private void MovePlayer()
    {
        //calculate movement direction
        moveDirection = hOrientation.forward * verticalInput + hOrientation.right * horizontalInput;
        if (_stats.States.Contains(PlayerStates.Dashing)) {
            speed = _stats.dashForce;
        } else
        if (_stats.States.Contains(PlayerStates.Sprinting) && energyManager.consumeEnergy(_stats.sprintCost))
        {
            speed = _stats.sprintSpeed;
        }
        else
        {
            speed = _stats.moveSpeed;
        }
        //on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * speed * 10f, ForceMode.Force);
        }
        //in Air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * speed * 10f * airMultiplier, ForceMode.Force);

        
    }
    private void Jump()
    {
        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        rb.AddForce(transform.up * _stats.jumpForce, ForceMode.Impulse);

    }

    private void Dash()
    {
        // reset x velocity
        //rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        _stats.States.Add(PlayerStates.Dashing);
        rb.AddForce(transform.forward * _stats.dashForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    private void ResetDash()
    {
        readyToDash = true;
    }
    private void ResetDashSpeed()
    {
        _stats.States.Remove(PlayerStates.Dashing);
    }
    private IEnumerator ResetJumpAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetJump();
        resetJumpRoutine = null; // Clear the reference when done
    }

    private IEnumerator ResetDashAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetDash();
        resetJumpRoutine = null; // Clear the reference when done
    }
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (_stats.States.Contains(PlayerStates.Dashing))//Dashing
        {
            speed = _stats.dashForce;
        }
        else
        if (_stats.States.Contains(PlayerStates.Sprinting))//Sprinting
        {
            speed = _stats.sprintSpeed;
        }
        else//Normal Walking
        {
            speed = _stats.moveSpeed;
        }
        if (flatVel.magnitude > speed)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
    // Update is called once per frame
    void Update()
    {
        //Rotate Player model
        transform.rotation = hOrientation.rotation;


        //ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        
        MyInput();
        SpeedControl();
        zoomMnger.speedZoom(speed);
        //Handle Drag
        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0;
        }
    }
}
