using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class FoxHealth : MonoBehaviour
{
    [Header("Assign in editor")]
    public SkinnedMeshRenderer foxMR;
    private Material hurtMaterial;

    public Animator sceneUIAnimation;

    public UnityEvent playerDeath;  // Event that informs Mist 
    public UnityEvent<int> playerHit; 

    [Space]
    [Header("Parameters")]
    public int health = 30;
    public float maxHurtMaterialValue;
    public FMODUnity.EventReference slowMoSnapshotEvent;

    private bool damaged = false;
    private Rigidbody foxRb;
    private FoxMovement foxMovement;
    private TimeManager timeManager;
    private FMOD.Studio.EventInstance slowMoSnapshot;
    private FMOD.Studio.PARAMETER_ID slowMoIntensityParameterId;

    private void Start()
    {
        hurtMaterial = foxMR.material;
        foxRb = GetComponent<Rigidbody>();
        foxMovement = GetComponent<FoxMovement>();

        timeManager = TimeManager.instance;

        // Get id for intensity parameter in SlowMotionEnemy Snapshot
        slowMoSnapshot = FMODUnity.RuntimeManager.CreateInstance(slowMoSnapshotEvent);
        FMOD.Studio.EventDescription slowMoEventDescription;
        slowMoSnapshot.getDescription(out slowMoEventDescription);

        FMOD.Studio.PARAMETER_DESCRIPTION slowMoParameterDescription;
        slowMoEventDescription.getParameterDescriptionByName("Intensity", out slowMoParameterDescription);

        slowMoIntensityParameterId = slowMoParameterDescription.id;
        slowMoSnapshot.start();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Damager") && !damaged && !foxMovement.isDashing)
        {
            damaged = true;
            health -= 10;    // Change this value depending on the damager

            playerHit.Invoke(health);

            if (health > 0)
            {
                HitAnimation();
                ManageHit(other);
            }
            else
            {
                ManageDeath();
            }
        }
    }

    private void ManageHit(Collider other)
    {
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
        Debug.Log("Dead");
        timeManager.PauseTime(1.5f);
        //timeManager.SmoothStopTime(0.1f, true, 1.4f);

        PlayUIOut();
        // Play particles and hide mesh

        // Reload everything
        Invoke("PrepareScene", 0.1f);
        Invoke("ReSpawnPlayer", 0.8f);

        //haha ha ckeado

        // ALSO MANAGE FOOD
    }

    private void PrepareScene() 
    {
        foxRb.position = foxMovement.spawn.position;
        foxRb.transform.rotation = Quaternion.Euler(0, 0, 0);
        playerDeath.Invoke();
    }

    private void PlayUIOut()
    {
        sceneUIAnimation.SetTrigger("Start");
    }

    private void PlayUIIn()
    {
        sceneUIAnimation.SetTrigger("End");
    }

    private void ReSpawnPlayer()
    {
        PlayUIIn();
        health = 30;
        damaged = false;
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
    }
    private void ChangeHurtValue(float x) 
    {
        hurtMaterial.SetFloat("_HurtValue", x);
    }

    private void SetSlowMoSnapshotIntensity(float x) 
    {
        slowMoSnapshot.setParameterByID(slowMoIntensityParameterId, x);
    }
}