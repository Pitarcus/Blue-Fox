using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using DG.Tweening;

public class EnemyWeakSpot : MonoBehaviour
{
    private Material dashResetMaterial;
    private MeshRenderer m_renderer;
    public Collider orbCollider;
    private ParticleSystem orbParticles;
    private ParticleSystem brokenOrbParticles;

    public FMODUnity.EventReference enemyHitEvent;
    public float explosionStrength;

    private bool invulnerable = true;
    private Rigidbody playerRb;
    private FoxMovement foxMovement;

    public UnityEvent onWeakSpotHit;

    void Start()
    {
        m_renderer = GetComponent<MeshRenderer>();
        dashResetMaterial = m_renderer.material;

        orbParticles = GetComponentInChildren<ParticleSystem>();
        brokenOrbParticles = transform.GetChild(1).GetComponent<ParticleSystem>();

        playerRb = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
        foxMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<FoxMovement>();

        orbParticles.Stop();

        orbCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !invulnerable && other.GetComponent<FoxMovement>().isDashing)
        {
            onWeakSpotHit.Invoke();

            invulnerable = true;
            EnemyHit();

            // Handle Player velocity and control to avoid undesired triggers
            foxMovement.canMove = false;
            playerRb.velocity = Vector3.zero;
            Invoke("EnablePlayerMovement", 0.2f);
            playerRb.AddExplosionForce(explosionStrength, transform.position, 10f);

            // Sound for hitting the weakspot
            FMODUnity.RuntimeManager.PlayOneShot(enemyHitEvent, transform.position);
            TimeManager.instance.PauseTime(0.05f);
        }
    }

    private void EnablePlayerMovement()
    {
        foxMovement.canMove = true;
    }

    // SetVulnerable runs when the enemy starts being vulnerable (weak animation)
    public void SetVulnerable()
    {
        invulnerable = false;
        orbCollider.enabled = true;
        orbParticles.Play();
    }

    public void SetInvulnerable()
    {
        invulnerable = true;
        orbCollider.enabled = false;
        orbParticles.Stop();
        m_renderer.enabled = true;
        DOVirtual.Float(0, 1f, 0.5f, SetOrbFresnel);
    }

    private void EnemyHit()
    {
        orbCollider.enabled = false;
        m_renderer.enabled = false;

        orbParticles.Stop();
        brokenOrbParticles.Play();

        DOVirtual.Float(1f, 0f, 0.2f, SetOrbFresnel);

        Invoke("SetInvulnerable", 0.3f);
    }

    private void SetOrbFresnel(float x)
    {
        dashResetMaterial.SetFloat("_FresnelAmount", x);
    }

    public void DestroyWeakSpot() 
    {
        DOVirtual.Float(1f, 0f, 0.2f, SetOrbFresnel);
        orbCollider.enabled = false;

        orbParticles.Stop();
        brokenOrbParticles.Play();
    }
}
