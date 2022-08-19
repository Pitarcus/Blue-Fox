using UnityEngine;
using System.Collections;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;
using DG.Tweening;
using Cinemachine;
using UnityEngine.VFX;
using UnityEngine.Rendering.Universal;
using UnityEngine.Events;

public class FoxMovement : MonoBehaviour
{
    [Header("Assign in Editor")]
    public Camera m_mainCamera;
    public CinemachineImpulseSource m_CinemachineImpulseSource;


    // Contraints for animation
    public Transform m_aimTarget;
    public MultiAimConstraint m_headConstraint;
    public Transform m_tailTarget;
    public ChainIKConstraint m_tailConstraint;

    public Rigidbody m_Rigidbody;   // For movement
    public LayerMask groundLayer;   // For the jump

    public Transform spawn;

    public ParticleSystem m_dashParticles;
    public VisualEffect dashWings;
    public VisualEffect jumpParticles;
    public SkinnedMeshRenderer foxMesh;

    // Event references for FMOD sounds
    public FMODUnity.EventReference dashSoundEvent; 
    public FMODUnity.EventReference dashResetEvent;
    public FMODUnity.EventReference jumpSoundEvent;
    public DecalProjector shadowDecal;

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
    public float coyoteTime = 0.25f;

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
    [HideInInspector]
    public Vector2 inputVector;
    private Vector3 m_Movement, desiredForward;
    private Quaternion m_Rotation = Quaternion.identity;
    [HideInInspector]
    public Vector3 forward, right; // Vectors to correct view and direction control
    [HideInInspector]
    public bool isMoving = false;   // Boolean to inform wether the player is moving or not (HealthUI)

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
    [HideInInspector]
    public bool playerCanJump = true;
    public bool playerCanDash = true;

    // Jump
    public bool isJumping = false;
    public bool isGrounded = true;
    public bool canJump = true;    // Variable that allows jump (coyote time implemented)
    private bool landing = false;
    private float jumpTimer = 0f;    // To check for how much the player is jumping
    private float maxShadowWidth = 3;
    private float maxShadowHeight = 6;
    private float coyoteTimer = 0f;

    // Dash
    [HideInInspector]
    public bool isDashing = false;
    private bool doDash = false;
    private bool canDash = true;
    private Material foxMaterial;
    /*private int fresnelAmountPropertyIndex; // POSSIBLE OPTIMIZATION OF MATERIAL ANIMATION
    private int fresnelPowerPropertyIndex;*/

    // Strawberries
    public UnityEvent onTrueGround;    // Event to inform Strawberries of true ground

    private void Awake()
    {
        input = new PlayerInput();
    }
    public void OnEnable()
    {
        // Character controls are enabled by default
        input.CharacterControls.Enable();
        input.CharacterControls.Movement.performed += OnMovementPerformed;
        input.CharacterControls.Movement.canceled += OnMovementCancelled;
        //input.CharacterControls.GetFood.performed += PressingFoodButton; // FOOD INPUT IS MANAGED BY BERRIES AND OTHERS

        input.CharacterControls.Jump.performed += JumpAction;

        input.CharacterControls.Jump.canceled += ctx => { isJumping = false; };

        input.CharacterControls.Dash.performed += DoDash;
    }

    public void OnDisable()
    {
        doDash = false; // When respawning, do not let dash play

        input.CharacterControls.Movement.performed -= OnMovementPerformed;
        input.CharacterControls.Movement.canceled -= OnMovementCancelled;

        input.CharacterControls.Jump.performed -= JumpAction;

        input.CharacterControls.Dash.performed -= DoDash;

        input.CharacterControls.Disable();
    }

    public void OnMovementPerformed(InputAction.CallbackContext context) 
    {
        inputVector = context.ReadValue<Vector2>();

        isMoving = true;
    }

    public void OnMovementCancelled(InputAction.CallbackContext context) 
    { 
        inputVector = Vector2.zero; 
        CalculateForwardVectors();

        isMoving = false;
    }

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();

        CalculateForwardVectors(); // Function that calculates the vectors for correcting movement depending on the camera's view

        foxMaterial = foxMesh.materials[0];
    }

    public void CalculateForwardVectors()   // Refresh the vectors depending on the view
    {
        forward = m_mainCamera.transform.forward;
        forward.y = 0;
        forward = forward.normalized;
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }

    void FixedUpdate()
    {
        // RAYCAST GROUND CHECK
        RaycastHit rayCastHit;
        /*if(Physics.Raycast(transform.position + transform.forward * 5 + new Vector3(-2, 5), Vector3.down, out rayCastHit, 5.1f, LayerMask.GetMask("Ground"))
            || Physics.Raycast(transform.position + transform.forward * 5 + new Vector3(2, 5), Vector3.down, out rayCastHit, 5.1f, LayerMask.GetMask("Ground"))
            || Physics.Raycast(transform.position + transform.forward * -5 + new Vector3(-2, 5), Vector3.down, out rayCastHit, 5.1f, LayerMask.GetMask("Ground"))
            || Physics.Raycast(transform.position + transform.forward * -5 + new Vector3(2, 5), Vector3.down, out rayCastHit, 5.1f, LayerMask.GetMask("Ground"))
            || Physics.Raycast(transform.position + new Vector3(0, 5), Vector3.down, out rayCastHit, 5.1f, LayerMask.GetMask("Ground"))) {*/
        if (Physics.SphereCast(transform.position + transform.forward * 4 + new Vector3(-2, 5), 1f, Vector3.down, out rayCastHit, 5f, LayerMask.GetMask("Ground"))
            || Physics.SphereCast(transform.position + transform.forward * 4 + new Vector3(2, 5), 1f, Vector3.down, out rayCastHit, 5f, LayerMask.GetMask("Ground"))
            || Physics.SphereCast(transform.position + transform.forward * -6 + new Vector3(-2, 5), 1f, Vector3.down, out rayCastHit, 5f, LayerMask.GetMask("Ground"))
            || Physics.SphereCast(transform.position + transform.forward * -6 + new Vector3(2, 5), 1f, Vector3.down, out rayCastHit, 5f, LayerMask.GetMask("Ground"))
            || Physics.SphereCast(transform.position + new Vector3(0, 5), 1f, Vector3.down, out rayCastHit, 5f, LayerMask.GetMask("Ground")))
        {
            if (!isGrounded)    // Ground enter
            {
                
                if (rayCastHit.collider.CompareTag("Ground_True"))
                {
                    onTrueGround.Invoke(); // Inform Strawberries
                }

                if (Mathf.Approximately(inputVector.magnitude, 0))
                {
                    m_Rigidbody.velocity = new Vector3 (0, m_Rigidbody.velocity.y, 0);
                }

                jumpParticles.Play();

                landing = false;

                if (!canDash)
                    canDashAnimation();

                canDash = true;
                dashSpeed -= airborneDashBonus;

                jumpTimer = 0f;

                coyoteTimer = 0f;

                shadowDecal.size = new Vector3(maxShadowWidth, maxShadowHeight, 15);
            }
            isGrounded = true;
            if(!isJumping)
                canJump = true;
            m_Animator.SetBool("isGrounded", isGrounded);
            m_Animator.applyRootMotion = true;
        }
        else
        {
            if (isGrounded)
            {

                if (!isJumping)
                    m_Animator.SetTrigger("falling");

                dashSpeed += airborneDashBonus;
            }
            isGrounded = false;
            m_Animator.SetBool("isGrounded", isGrounded);
            m_Animator.applyRootMotion = false;

        }

        if (!isDashing && canMove)  // only move when enabled and is not dashing
        {
            HandleMovement();
            HandleJump();
            if (doDash && canDash)
                HandleDash();
        }
        else if (!canMove) // When eating food, watch for animations and stuff (force idle)
        {
            if (!isJumping && !isDashing && canJump)
            {
                m_aimTarget.localPosition = new Vector3(0, m_aimTarget.localPosition.y, m_aimTarget.localPosition.z);
                m_Animator.SetBool("IsWalking", false);
                m_Animator.SetFloat("speed", 0f);
                m_Rigidbody.velocity = Vector3.zero;
                m_Rigidbody.angularVelocity = Vector3.zero;
            }
        }
    }

    private void Update()
    {
        if(!isGrounded) // Can jump doesnt change until coyote time has passed or player has jumped
        {
            if (canJump)
            {
                coyoteTimer += Time.deltaTime;
                if (coyoteTimer > coyoteTime)
                {
                    canJump = false;
                }
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

        if (!isJumping && !isDashing && canJump)
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
            m_tailConstraint.weight = 1;
            ManageHeadAim();
        }
        else
        {
            headIsTurningLeft = false;
            headIsTurningRight = false;
            movingTimer = 0;
            stillTimer += Time.deltaTime;
            if (stillTimer >= m_headTurnTime)
            {
                m_headConstraint.weight = 0;
                m_tailConstraint.weight = 0;
                return;
            }
            else
            {
                m_aimTarget.localPosition = Vector3.Lerp(m_aimTarget.localPosition, new Vector3(0, m_aimTarget.localPosition.y, m_aimTarget.localPosition.z), stillTimer / m_headTurnTime);
                m_tailTarget.localPosition = Vector3.Lerp(m_tailTarget.localPosition, new Vector3(0, m_tailTarget.localPosition.y, m_tailTarget.localPosition.z), stillTimer / m_headTurnTime);
            }
        }

    }
    void ManageHeadAim()
    {
        // Timers for the duration of the head aim interpolation (in the lerp functions)
        stillTimer = 0;

        movingTimer += Time.deltaTime;

        // Move the target's local X according to the direction the fox is facing
        if (Mathf.Approximately(m_Rotation.eulerAngles.y, transform.rotation.eulerAngles.y))  // When there is no rotation left target to x = 0 (centered)
        {
            if ((headIsTurningRight || headIsTurningLeft))
            {
                movingTimer = 0;
                headIsTurningLeft = false;
                headIsTurningRight = false;
            }
            m_aimTarget.localPosition = Vector3.Lerp(m_aimTarget.localPosition, new Vector3(0, m_aimTarget.localPosition.y, m_aimTarget.localPosition.z), movingTimer / m_headTurnTime);
            m_tailTarget.localPosition = Vector3.Lerp(m_tailTarget.localPosition, new Vector3(0, m_tailTarget.localPosition.y, m_tailTarget.localPosition.z), movingTimer / m_headTurnTime);
        }

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
            m_tailTarget.localPosition = Vector3.Lerp(m_tailTarget.localPosition, new Vector3(m_maxAimTargetX, m_tailTarget.localPosition.y, m_tailTarget.localPosition.z), movingTimer / m_headTurnTime);
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
            m_tailTarget.localPosition = Vector3.Lerp(m_tailTarget.localPosition, new Vector3(m_minAimTargetX, m_tailTarget.localPosition.y, m_tailTarget.localPosition.z), movingTimer / m_headTurnTime);
        }

        
    }
    void JumpAction(InputAction.CallbackContext context) 
    {
        if (!isDashing && canJump && canMove)
        {
            FMODUnity.RuntimeManager.PlayOneShot(jumpSoundEvent);

            m_Animator.ResetTrigger("landing");
            m_Animator.applyRootMotion = false;

            m_Rigidbody.drag = 0;

            if (!isGrounded) // In case of a coyote jump, reset Y velocity
                m_Rigidbody.velocity = new Vector3(m_Rigidbody.velocity.x, 0, m_Rigidbody.velocity.z);
            m_Rigidbody.velocity += Vector3.up * jumpVelocity;

            isJumping = true;
            //isGrounded = false;
            canJump = false;

            jumpParticles.Play();

            m_Animator.SetTrigger("jump");
            //m_Animator.SetBool("isGrounded", isGrounded);
        }
    }
    public void JumpActionAuto()
    {
        if (!isDashing && isGrounded && canMove)
        {
            m_Animator.ResetTrigger("landing");
            m_Animator.applyRootMotion = false;

            m_Rigidbody.drag = 0;
            m_Rigidbody.velocity += Vector3.up * jumpVelocity;

            isJumping = true;
            //isGrounded = false;
            canJump = false;

            jumpParticles.Play();

            m_Animator.SetTrigger("jump");
            //m_Animator.SetBool("isGrounded", isGrounded);
        }
    }
    void HandleJump() // Manage airborne movement
    {
        if (!isGrounded && playerCanJump) // Only handle jump when airborne
        {
            if (jumpTimer < maxJumpTime)
                jumpTimer += Time.deltaTime;
           
            m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * jumpMovementSpeed);

            if (m_Rigidbody.velocity.y < fallSpeedThreshold && !canJump)    // Chacter is falling after max height
            {   
                m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (m_Rigidbody.velocity.y > fallSpeedThreshold && !canJump && (!isJumping || jumpTimer >= maxJumpTime) ) // Player released the jump button mid-jump
            {
                m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }

            if(!landing)
                CheckLandingDistance();

            // Modify shadow decal under the player
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, 50, groundLayer);
            shadowDecal.size = new Vector3(maxShadowWidth - maxShadowWidth * hit.distance / 20, maxShadowHeight - maxShadowHeight * hit.distance / 20, 15);
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
        if (!isDashing && playerCanDash)
        {
            doDash = true;
        }
    }
    void HandleDash() // Dash motion
    {
        // Booleans
        canDash = false;
        doDash = false;
        isDashing = true;

        FMODUnity.RuntimeManager.PlayOneShot(dashSoundEvent);
        // Camera shake
        m_CinemachineImpulseSource.GenerateImpulse();

        // Dash Particles
        m_dashParticles.Play();
        dashWings.Play();

        // Animator stuff (change the animation to jump for better movement)
        m_Animator.SetBool("dashing", true);
        m_Animator.applyRootMotion = false;

        // Stop all velocities (stop body)
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;

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

    /*private void OnCollisionEnter(Collision collision)  // Getting the collision with the ground objects (player is grounded)
    {
        
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Ground_True")) 
        {
            if (collision.gameObject.CompareTag("Ground_True"))
            {
                onTrueGround.Invoke(); // Inform Strawberries
            }

            if (Mathf.Approximately(inputVector.magnitude, 0))
            {
                m_Rigidbody.velocity = Vector3.zero;
            }

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

            shadowDecal.size = new Vector3(maxShadowWidth, maxShadowHeight, 15);
        }
    }*/

    /*private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Ground_True"))
        {
            // Check if is still colliding, if it is, dont do what it should when taking off
            if (!Physics.Raycast(transform.position + transform.forward * 5 + new Vector3(0, 5), Vector3.down, 5.5f, LayerMask.GetMask("Ground")))
            {
                if (m_Rigidbody.velocity.y != 0)
                    isGrounded = false;

                m_Animator.SetBool("isGrounded", isGrounded);
                m_Animator.applyRootMotion = false;
                if (!isJumping)
                    m_Animator.SetTrigger("falling");

                dashSpeed += airborneDashBonus;
            }
            else // If not really airborne
            {
                dashSpeed += airborneDashBonus;
            }
        }
    }*/

    private void OnTriggerEnter(Collider other) 
    {
        if (other.gameObject.CompareTag("DashReset")) 
        {
            FMODUnity.RuntimeManager.PlayOneShot(dashResetEvent);
            m_CinemachineImpulseSource.GenerateImpulse(0.6f);
            if (!canDash)
                canDashAnimation();
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
        if (!isDashing)
            FMODUnity.RuntimeManager.PlayOneShot(dashResetEvent);
        DOVirtual.Float(1f, 0f, 0.05f, SetMaterialFresnelAmount);
    }
}
