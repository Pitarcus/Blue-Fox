using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using Cinemachine;
using System.Collections;

public class FoxHealth : MonoBehaviour
{
    [Header("Assign in editor")]
    public SkinnedMeshRenderer foxMR;
    public ParticleSystem deathParticles;
    private Material hurtMaterial;

    public Animator sceneUIAnimation;

    public UnityEvent playerDeath;  // Event that informs Mist & Foodbag & Strawberries
    public UnityEvent<int> playerHit;

    public UnityEvent playerRespawned;

    public UnityEngine.Rendering.Volume hurtVolume;

    public FMODUnity.EventReference foxHurtSoundEvent;
    public FMODUnity.EventReference foxDeadEvent;

    public TutorialTriggerText foodLostTutorialTriggerText;


    [Space]
    [Header("Parameters")]
    public int health = 30;
    public float maxHurtMaterialValue;
    public FMODUnity.EventReference slowMoSnapshotEvent;

    [HideInInspector] public Vector3 lastGroundPosition;
    private bool damaged = false;   // invulnerability boolean
    private bool dead = false;
    private Rigidbody foxRb;
    private FoxMovement foxMovement;
    private TimeManager timeManager;
    private FMOD.Studio.EventInstance slowMoSnapshot;
    private FMOD.Studio.PARAMETER_ID slowMoIntensityParameterId;

    private CinemachineImpulseSource m_CinemachineImpulseSource;

    private bool hasDiedOnce = false;

    private void Awake()
    {
        hurtMaterial = foxMR.material;
        foxRb = GetComponent<Rigidbody>();
        foxMovement = GetComponent<FoxMovement>();

        // Get id for intensity parameter in SlowMotionEnemy Snapshot
        slowMoSnapshot = FMODUnity.RuntimeManager.CreateInstance(slowMoSnapshotEvent);
        FMOD.Studio.EventDescription slowMoEventDescription;
        slowMoSnapshot.getDescription(out slowMoEventDescription);

        FMOD.Studio.PARAMETER_DESCRIPTION slowMoParameterDescription;
        slowMoEventDescription.getParameterDescriptionByName("Intensity", out slowMoParameterDescription);

        slowMoIntensityParameterId = slowMoParameterDescription.id;


        m_CinemachineImpulseSource = foxMovement.m_CinemachineImpulseSource;
    }

    private void Start()
    {
        timeManager = TimeManager.instance;
       
        slowMoSnapshot.start();

        CinemachineImpulseManager.Instance.IgnoreTimeScale = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damager") && !damaged && !foxMovement.isDashing)
        {
            damaged = true;
            health -= 10;    // Change this value depending on the damager

            playerHit.Invoke(health);

            m_CinemachineImpulseSource.GenerateImpulse();

            if (health > 0)
            {
                HitAnimation();
                ManageHit(other);
            }
            else
            {
                if(!dead)
                    ManageDeath();
            }
        }
        else if (other.gameObject.CompareTag("FallDeath"))
        {
            if(!dead)
                ManageDeath();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if(collision.collider.CompareTag("Ground") || collision.gameObject.CompareTag("Ground_True"))
        {
            lastGroundPosition = transform.position;
        }
    }

    private void ManageHit(Collider other)
    {
        FMODUnity.RuntimeManager.PlayOneShot(foxHurtSoundEvent);

        // Explosion
        Vector3 forceDirection = transform.position - new Vector3(other.transform.position.x, transform.position.y, other.transform.position.z);
        forceDirection = forceDirection.normalized;
        foxRb.AddForce(forceDirection * 1000f);// Change this force for one that comes from the damager object

        // Player movement & forces
        if (foxMovement.isJumping)
            foxRb.velocity = new Vector3(foxRb.velocity.x, foxRb.velocity.y * 0.5f, foxRb.velocity.z);

        foxMovement.JumpActionAuto();

        foxMovement.canMove = false;
        Invoke("EnableMovement", 0.3f);
    }

    void ManageDeath()
    {
        dead = true;

        if(!hasDiedOnce)
        {
            foodLostTutorialTriggerText.ShowTextSequence();
        }

        hasDiedOnce = true;

        FMODUnity.RuntimeManager.PlayOneShot(foxDeadEvent);

        foxMovement.m_CinemachineImpulseSource.GenerateImpulse();
        timeManager.PauseTime(1.5f);

        PlayUIOut();
        
        // Play particles and hide mesh
        deathParticles.Play();
        foxMovement.foxMesh.enabled = false;

        playerDeath.Invoke();
        // Reload everything
        //Invoke("ReSpawnPlayer", 0.5f);
        StartCoroutine(ReSpawnPlayer(true, 2f));

        // ALSO MANAGE FOOD
    }

    private void PlayUIOut()
    {
        sceneUIAnimation.SetTrigger("Start");
    }

    private void PlayUIIn()
    {
        sceneUIAnimation.SetTrigger("End");
    }

    public IEnumerator ReSpawnPlayer(bool triggerUI, float delayTime)
    {
        yield return new WaitForSecondsRealtime(delayTime);

        foxRb.position = foxMovement.spawn.position;
        foxMovement.foxMesh.enabled = true;
        foxMovement.m_Rigidbody.velocity = Vector3.zero;
        foxMovement.isGrounded = false;
        foxRb.transform.rotation = Quaternion.Euler(0, 0, 0);
        if(triggerUI)
            PlayUIIn();
        health = 30;
        damaged = false;
        dead = false;

        playerRespawned.Invoke();
    }

    private void EnableMovement()
    {
        foxMovement.canMove = true;
        damaged = false;
        foxMovement.isJumping = false;
    }

    private void HitAnimation() 
    {
        // Color of the player animation
        Sequence hitAnimation = DOTween.Sequence();
        hitAnimation.Append(DOVirtual.Float(0, maxHurtMaterialValue, 0.1f, ChangeHurtValue));
        hitAnimation.Append(DOVirtual.Float(maxHurtMaterialValue, 0, 0.5f, ChangeHurtValue));

        // Slow motion time animation
        timeManager.SmoothStopTime(0.1f, true, 0.8f);

        // Slow motion sound
        Sequence slowSound = DOTween.Sequence();
        slowSound.Append(DOVirtual.Float(0f, 100f, 0.3f, SetSlowMoSnapshotIntensity));
        slowSound.Append(DOVirtual.Float(100f, 0f, 0.6f, SetSlowMoSnapshotIntensity));

        // Post Processing
        DOVirtual.Float(1, 0, 0.8f, ChangeVolumeIntensity);
    }
    private void ChangeHurtValue(float x) 
    {
        hurtMaterial.SetFloat("_HurtValue", x);
    }

    private void SetSlowMoSnapshotIntensity(float x) 
    {
        slowMoSnapshot.setParameterByID(slowMoIntensityParameterId, x);
    }

    private void ChangeVolumeIntensity(float x)
    {
        hurtVolume.weight = x;
    }

}