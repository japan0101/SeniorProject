using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public float sprintSpeed;
    public float sprintCost;

    private float speed;

    public float groundDrag;

    public float dashForce;
    public float dashDuration;
    public float dashCooldown;
    public float dashCost;
    bool dashing;
    bool readyToDash;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float jumpCost;
    bool readyToJump;

    [Header("References")]
    public EnergyManager energyManager;
    public Camera playerCam;

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
        if (Input.GetKey(jumpKey) && readyToJump && grounded && energyManager.consumeEnergy(jumpCost))
        {
            readyToJump = false;
            Jump();

            // If there's an existing coroutine, stop it first
            if (resetJumpRoutine != null)
            {
                StopCoroutine(resetJumpRoutine);
            }

            resetJumpRoutine = StartCoroutine(ResetJumpAfterDelay(jumpCooldown));
        }

        //when dash
        if (Input.GetKey(dashKey) && readyToDash && energyManager.consumeEnergy(dashCost))
        {
            readyToDash = false;
            dashing = true;
            Dash();

            Invoke(nameof(ResetDashSpeed), dashDuration);

            // If there's an existing coroutine, stop it first
            if (resetDashRoutine != null)
            {
                StopCoroutine(resetDashRoutine);
            }

            resetDashRoutine = StartCoroutine(ResetDashAfterDelay(dashCooldown));
        }
    }

    private void MovePlayer()
    {
        //calculate movement direction
        moveDirection = hOrientation.forward * verticalInput + hOrientation.right * horizontalInput;
        if (dashing) {
            speed = dashForce;
        } else
        if (Input.GetKey(sprintKey) && energyManager.consumeEnergy(sprintCost))
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = moveSpeed;
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

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

    private void Dash()
    {
        // reset x velocity
        //rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);

        rb.AddForce(transform.forward * dashForce, ForceMode.Impulse);
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
        dashing = false;
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
        if (dashing)
        {
            speed = dashForce;
        }
        else
        if (Input.GetKey(sprintKey))
        {
            speed = sprintSpeed;
        }
        else
        {
            speed = moveSpeed;
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
