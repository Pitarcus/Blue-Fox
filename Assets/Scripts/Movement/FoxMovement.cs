using UnityEngine;
using System.Collections;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using DG.Tweening;
using Cinemachine;
using UnityEngine.VFX;

public class FoxMovement : MonoBehaviour
{
    [Header("Assign in Editor")]
    public Camera m_mainCamera;
    public CinemachineImpulseSource m_CinemachineImpulseSource;
    public Transform m_aimTarget;
    public MultiAimConstraint m_headConstraint;
    public Rigidbody m_Rigidbody;   // For movement
    public LayerMask groundLayer;   // For the jump
    public Transform spawn;
    public ParticleSystem m_dashParticles;
    public VisualEffect dashWings;
    public VisualEffect jumpParticles;
    public SkinnedMeshRenderer foxMesh;
    

    [Space]
    [Header("Movement Parameters")]
    public float m_turnSpeed = 20f;

    [Space]
    [Header("Head Aim Parameters")]
    public float m_minAimTargetX = -5;
    public float m_maxAimTargetX = 5;
    public float m_headTurnTime = 0.5f;

    [Space]
    [Header("Jump Parameters")]
    public float jumpVelocity = 10f;
    public float jumpMovementSpeed = 10f;
    public float fallSpeedThreshold = 1f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float landingDistanceFromGround = 5f;
    public float maxJumpTime = 2.5f;

    [Space]
    [Header("Dash Parameters")]
    public float dashSpeed = 100;
    public float dashMaxDrag = 14f;
    public float airborneDashBonus = 25f;
    [Header("Dash Animation")]
    public float dashFresnelInitAmount = 1f;
    public float dashInitPowerAmount = 0f;
    public float dashFinalPowerAmount = 1f;
    public float dashFresnelTime = 0.5f;

    // Movement
    private Animator m_Animator;
    private Vector2 inputVector;
    private Vector3 m_Movement, desiredForward;
    private Quaternion m_Rotation = Quaternion.identity;
    private Vector3 forward, right;

    // Head aim
    private float movingTimer = 0f;
    private float stillTimer = 0f;
    private bool headIsTurningRight = false;
    private bool headIsTurningLeft = false;

    // Input
    [HideInInspector]
    public PlayerInput input;

    // Movement enabler/disabler
    [HideInInspector]
    public bool canMove = true;

    // Jump
    public bool isJumping = false;
    public bool isGrounded = true;
    private bool landing = false;
    private float jumpTimer = 0f;    // To check for how much the player is jumping

    // Dash
    public bool isDashing = false;
    public bool doDash = false;
    public bool canDash = true;
    private Material foxMaterial;

    private void OnEnable()
    {
        input = new PlayerInput();

        // Character controls are enabled by default
        input.CharacterControls.Enable();
        input.CharacterControls.Movement.performed += ctx => { inputVector = ctx.ReadValue<Vector2>(); };
        input.CharacterControls.Movement.canceled += ctx => inputVector = Vector2.zero;
        //input.CharacterControls.GetFood.performed += PressingFoodButton;

        input.CharacterControls.Jump.performed += JumpAction;

        input.CharacterControls.Jump.canceled += ctx => { isJumping = false; };

        input.CharacterControls.Dash.performed += DoDash;
    }

    private void OnDisable()
    {
        input.CharacterControls.Jump.performed -= JumpAction;

        input.CharacterControls.Dash.performed -= DoDash;

        input.CharacterControls.Disable();
    }

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();

        forward = m_mainCamera.transform.forward;
        forward.y = 0;
        forward = forward.normalized;
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;

        foxMaterial = foxMesh.materials[0];
    }

    void FixedUpdate()
    {
        if (!isDashing && canMove)  // only move when enabled and is not dashing
        {
            HandleMovement();
            HandleJump();
            if (doDash && canDash)
                HandleDash();
        }
        else if (!canMove) // When eating food, watch for animations and stuff
        {
            if (!isJumping && !isDashing && isGrounded)
            {
                m_aimTarget.localPosition = new Vector3(0, m_aimTarget.localPosition.y, m_aimTarget.localPosition.z);
                m_Animator.SetBool("IsWalking", false);
                m_Animator.SetFloat("speed", 0f);
                m_Rigidbody.velocity = Vector3.zero;
            }
        }
       
    }

    private void HandleMovement() 
    {
        float horizontal = inputVector.x;
        float vertical = inputVector.y;

        // Corrected vectors for "isometric" view
        Vector3 horizontalMovement = horizontal * right;
        Vector3 verticalMovement = vertical * forward;

        m_Movement.Set(horizontal, 0f, vertical);
        m_Movement = horizontalMovement + verticalMovement;
        m_Movement.Normalize();

        // Booleans for animations and logic
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;

        if (!isJumping && !isDashing && isGrounded)
        {
            m_Animator.SetBool("IsWalking", isWalking);
            m_Animator.SetFloat("speed", new Vector2(Mathf.Abs(horizontal), Mathf.Abs(vertical)).magnitude);
        }

        // Direction we actually want
        desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, m_turnSpeed * Time.deltaTime, 0f);
        m_Rotation = Quaternion.LookRotation(desiredForward);

        // Manage the head target when moving horizontally 
        if (isWalking)
        {
            m_headConstraint.weight = 1;
            ManageHeadAim();
        }
        else
        {
            m_headConstraint.weight = 0;
            movingTimer = 0;
            stillTimer += Time.deltaTime;
            if (stillTimer == m_headTurnTime)
                return;
            else
            {
                m_aimTarget.localPosition = Vector3.Lerp(m_aimTarget.localPosition, new Vector3(0, m_aimTarget.localPosition.y, m_aimTarget.localPosition.z), stillTimer / m_headTurnTime);
            }
        }

    }
    void ManageHeadAim()
    {
        // Timers for the duration of the head aim interpolation (in the lerp functions)
        stillTimer = 0;

        movingTimer += Time.deltaTime;

        if (movingTimer == m_headTurnTime)
        {
            return;
        }
        // Move the target's local X according to the direction the fox is facing
        else if (m_Rotation.eulerAngles.y > transform.rotation.eulerAngles.y) // When rotating to the right target to x = maxAimTargetX
        {
            if (!headIsTurningRight)    // If there was another movement, reset the timer (and position of the head)
            {
                movingTimer = 0;
                headIsTurningRight = true;
            }
            if (headIsTurningLeft)
            {
                movingTimer = 0;
                headIsTurningLeft = false;
            }
            m_aimTarget.localPosition = Vector3.Lerp(m_aimTarget.localPosition, new Vector3(m_maxAimTargetX, m_aimTarget.localPosition.y, m_aimTarget.localPosition.z), movingTimer / m_headTurnTime);
        }

        else if (m_Rotation.eulerAngles.y < transform.rotation.eulerAngles.y) // When rotating to the left target to x = minAimTargetX
        {
            if (!headIsTurningLeft)
            {
                movingTimer = 0;
                headIsTurningLeft = true;
            }

            if (headIsTurningRight)
            {
                movingTimer = 0;
                headIsTurningRight = false;
            }
            m_aimTarget.localPosition = Vector3.Lerp(m_aimTarget.localPosition, new Vector3(m_minAimTargetX, m_aimTarget.localPosition.y, m_aimTarget.localPosition.z), movingTimer / m_headTurnTime);
        }

        else if (m_Rotation.eulerAngles.y == transform.rotation.eulerAngles.y)  // When there is no rotation left target to x = 0 (centered)
        {
            if (headIsTurningRight || headIsTurningLeft)
            {
                movingTimer = 0;
                headIsTurningLeft = false;
                headIsTurningRight = false;
            }
            m_aimTarget.localPosition = Vector3.Lerp(m_aimTarget.localPosition, new Vector3(0, m_aimTarget.localPosition.y, m_aimTarget.localPosition.z), movingTimer / m_headTurnTime);
        }
    }
    void JumpAction(InputAction.CallbackContext context) 
    {
        if (!isDashing && isGrounded && canMove)
        {
            m_Rigidbody.drag = 0;
            m_Animator.applyRootMotion = false;
            isJumping = true;
            m_Rigidbody.velocity += Vector3.up * jumpVelocity;
            isGrounded = false;

            jumpParticles.Play();

            m_Animator.SetTrigger("jump");
            m_Animator.SetBool("isGrounded", isGrounded);
        }
    }
    void HandleJump() 
    {
        if (!isGrounded) // Only handle jump when airborne
        {
            if (jumpTimer < maxJumpTime)
                jumpTimer += Time.deltaTime;
            Debug.Log(jumpTimer);
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * jumpMovementSpeed);
            if (m_Rigidbody.velocity.y < fallSpeedThreshold)    // Chacter is falling after max height
            {   
                m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (m_Rigidbody.velocity.y > fallSpeedThreshold && (!isJumping || jumpTimer >= maxJumpTime)) // Player released the jump button mid-jump
            {
                m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }

            if (!landing)
            CheckLandingDistance();
        }
    }
    void CheckLandingDistance() // Check for the distance between the player and the ground, to trigger the landing animation
    {
        RaycastHit hit;
        bool raycasted = Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer);
        if (raycasted)
        {
            if ( m_Rigidbody.velocity.y <= 0 && hit.distance <= landingDistanceFromGround)
            {
                m_Animator.SetTrigger("landing");
                landing = true;
            }
        }
    }

    void DoDash(InputAction.CallbackContext context) 
    {
        if (!isDashing)
            doDash = true;
    }
    void HandleDash() // Dash motion
    {
        // Booleans
        canDash = false;
        doDash = false;
        isDashing = true;

        // Camera shake
        m_CinemachineImpulseSource.GenerateImpulse();

        // Dash Particles
        m_dashParticles.Play();
        dashWings.Play();

        // Animator stuff (change the animation to jump for better movement)
        m_Animator.SetBool("dashing", true);
        m_Animator.applyRootMotion = false;

        // Actual forces calculation
        m_Rigidbody.velocity = Vector3.zero;

        Vector3 dir;
        if (m_Movement != Vector3.zero)
        {
            dir = m_Movement;
            m_Rotation = Quaternion.LookRotation(dir);
        }
        else
            dir = transform.forward;

        m_Rigidbody.velocity += dir.normalized * dashSpeed;

        m_Rigidbody.drag = dashMaxDrag;
        m_Rigidbody.useGravity = false;

        StartCoroutine(DashWait());
    }

    IEnumerator DashWait() 
    {
        // Tween for rigidbody's drag over dash time
        DOVirtual.Float(dashMaxDrag, 0f, .5f, SetRigidbodyDrag);
        DOVirtual.Float(dashFresnelInitAmount, 0.5f, dashFresnelTime, SetMaterialFresnelAmount);
        DOVirtual.Float(dashInitPowerAmount, dashFinalPowerAmount, dashFresnelTime, SetMaterialFesnelPower);

        yield return new WaitForSeconds(.3f);

        m_Rigidbody.useGravity = true;
        m_Rigidbody.drag = 0;

        // Animator stuff
        m_Animator.applyRootMotion = true;
        m_Animator.SetBool("dashing", false);

        m_dashParticles.Stop();

        // Booleans
        isDashing = false;
        if (isGrounded)
        {
            Invoke("canDashAnimation", 0.2f);
            canDash = true;
            m_Rigidbody.velocity = Vector3.zero;
        }
    }
    private void SetRigidbodyDrag(float x) 
    {
            m_Rigidbody.drag = x;
    }
    private void SetMaterialFresnelAmount(float x)
    {
        foxMaterial.SetFloat("_FresnelAmount", x);
    }
    private void SetMaterialFesnelPower(float x) 
    {
        foxMaterial.SetFloat("_FresnelPower", x);
    }
    
    void OnAnimatorMove()   // Handle movement alongside the root motion applied
    {

        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);

        m_Rigidbody.MoveRotation(m_Rotation);
    }

    private void OnCollisionEnter(Collision collision)  // Getting the collision with the ground objects (player is grounded)
    {
        
        if (collision.gameObject.CompareTag("Ground")) 
        {
            isGrounded = true;

            m_Animator.SetBool("isGrounded", isGrounded);
            m_Animator.applyRootMotion = true;

            jumpParticles.Play();

            landing = false;
            if (!canDash)
                canDashAnimation();
            canDash = true;
            dashSpeed -= airborneDashBonus;

            jumpTimer = 0f;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if(m_Rigidbody.velocity.y != 0)
                isGrounded = false;
            m_Animator.SetBool("isGrounded", isGrounded);
            m_Animator.applyRootMotion = false;
            if (!isJumping)
                m_Animator.SetTrigger("falling");
            dashSpeed += airborneDashBonus;
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("DashReset")) 
        {
            m_CinemachineImpulseSource.GenerateImpulse(0.6f);
            if (!canDash)
                canDashAnimation();
            canDash = true;
            
        }
        else if (other.gameObject.CompareTag("FallDeath")) 
        {
            transform.position = spawn.position;
            canDash = true;
        }
    }
    private void canDashAnimation() 
    {
        Tween fresnelAmount = DOVirtual.Float(0f, 1f, 0.2f, SetMaterialFresnelAmount);
        DOVirtual.Float(1f, -0.5f, 0.3f, SetMaterialFesnelPower);

        fresnelAmount.OnComplete(resetFresnel);
    }
    private void resetFresnel()
    {
        DOVirtual.Float(1f, 0f, 0.05f, SetMaterialFresnelAmount);
    }

    /*private void PressingFoodButton(InputAction.CallbackContext ctx) 
    {

    }*/
}
