using FMODUnity;
using UnityEngine;
using DG.Tweening;

public class ThrowingPlatform : MonoBehaviour
{

    // Parameters

    public bool fallingPlatform;
    
    [Header("COMMON")]
    public Ease easeType;
    public bool resetPosition = true;
    [Tooltip("Mesh that is child of this object that shows the shake")] public Transform mesh;
    public float duration = 2f;
    public float jumpVelocityMultiplier;
    public float shakeDuration = 0.3f;
    public float shakeStrenght = 1f;
    public int shakeVibrato = 10;

    [Header("Falling platforms")]
    public FMODUnity.EventReference fallingPlatformSoundEvent;
    public ParticleSystem fallingPlatformParticles;

    [Header("Throwing platforms")]
    public FMODUnity.EventReference throwingPlatformSoundEvent;
    [Tooltip("On Not Falling Platforms")]
    public Transform targetPosition;
    [Tooltip("On Not Falling Platforms")]
    public Animator throwingPlatformClogs;
    public MeshRenderer lightMeshRenderer;
    public Color redColor;
    public Color greenColor;
    public Color yellowColor;

    private Material lightMaterial;

    // Platform members
    private Vector3 originalPosition;
    private Vector3 movingDirection;
    private bool onOrigin = true;
    private float jumpVelocity;  // the bonus of the jump when leaving the platform
    private bool onPlatform = false;

    // References
    private GameObject player;
    private Rigidbody playerRb;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        if (!fallingPlatform)
        {
            originalPosition = transform.position;
            movingDirection = targetPosition.position - transform.position;
            movingDirection.Normalize();

            lightMaterial = lightMeshRenderer.material;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (onOrigin)
        {
            if (fallingPlatform)
                FallingPlatform();
            else
            {
                ThrowPlatform();
            }
        }
    }

    void ChangeJumpVelocity(float x) 
    {
        jumpVelocity = x;
    }

    void OnOrigin() 
    {
        onOrigin = true;

        lightMaterial.SetColor("_EmissionColor", redColor);

        if (onPlatform)
        {
            if (fallingPlatform)
                FallingPlatform();
            else
            {
                ThrowPlatform();
            }
        }
    }

    private void ThrowPlatform() 
    {
        //FMODUnity.RuntimeManager.PlayOneShot(throwingPlatformSoundEvent, transform.position);

        throwingPlatformClogs.SetTrigger("Move");

        lightMaterial.SetColor("_EmissionColor", greenColor);

        // Create the sequence of the movement of the platform
        Sequence movementSequence = DOTween.Sequence();

        movementSequence.Append(mesh.DOShakePosition(shakeDuration, shakeStrenght, shakeVibrato));   // May change for a child mesh
        movementSequence.Join(transform.DOMove(targetPosition.position, duration, false).SetEase(easeType));    // Throw movement

        if (resetPosition)
        {
            movementSequence.AppendInterval(1f);
            movementSequence.Append(transform.DOMove(originalPosition, duration + 1f, false)
                .OnPlay(() => lightMaterial.SetColor("_EmissionColor", yellowColor))).OnComplete(OnOrigin);
        }

        // Create the sequence of the values for the inertia of the player jump
        Sequence jumpSequence = DOTween.Sequence();
        jumpSequence.Append(DOVirtual.Float(0f, 1f, duration, ChangeJumpVelocity).SetEase(easeType));
        jumpSequence.AppendInterval(0.1f);
        jumpSequence.Append(DOVirtual.Float(1f, 0f, 0.3f, ChangeJumpVelocity));
        //DOVirtual.Float(0f, 0.9f, duration + 0.2f, ChangeJumpVelocity).SetEase(Ease.InOutElastic).OnComplete(ResetJumpVelocity);

        onOrigin = false;
    }

    void SetPlatformColor(Color color)
    {
        lightMaterial.SetColor("_EmissionColor", color);
    }
    private void FallingPlatform() 
    {
        FMODUnity.RuntimeManager.PlayOneShot(fallingPlatformSoundEvent, transform.position);

        fallingPlatformParticles.Play();

        // Create the sequence of the movement of the platform
        Sequence movementSequence = DOTween.Sequence();

        movementSequence.Append(mesh.DOShakePosition(shakeDuration, shakeStrenght, shakeVibrato));
        movementSequence.AppendInterval(0.5f);
        movementSequence.Join(transform.DOMove(transform.position - new Vector3(0, 300), duration, false)).SetEase(Ease.InQuart);

        if (resetPosition)
        {
            movementSequence.AppendInterval(1f);
            movementSequence.Append(transform.DOMove(originalPosition, duration + 5f, false)).OnComplete(OnOrigin);
        }
        onOrigin = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlatform = true;
            if (player == null)
            {
                player = other.gameObject;
                playerRb = player.GetComponent<Rigidbody>();
            }
            if (!fallingPlatform)
            {
                player.transform.parent = transform;
                Physics.SyncTransforms();
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onPlatform = false;
            player.transform.parent = null;
            playerRb.velocity += movingDirection * jumpVelocity * jumpVelocityMultiplier;
        }
    }
}

