using UnityEngine;
using DG.Tweening;

public class DashResetOrb : MonoBehaviour
{
    private Material dashResetMaterial;
    private MeshRenderer m_renderer;
    private Collider orbCollider;
    private ParticleSystem orbParticles;
    private ParticleSystem brokenOrbParticles;
    public FMODUnity.EventReference breakingOrbEvent;

    void Start()
    {
        m_renderer = GetComponent<MeshRenderer>();
        dashResetMaterial = m_renderer.material;
        orbCollider = GetComponent<Collider>();

        orbParticles = GetComponentInChildren<ParticleSystem>();
        brokenOrbParticles = transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        FMODUnity.RuntimeManager.PlayOneShot(breakingOrbEvent, transform.position);
        TimeManager.instance.PauseTime(0.05f);
        AnimateDashReseter();
        Invoke("EnableDashResetObject", 1.5f);
    }
    private void AnimateDashReseter()
    {
        orbCollider.enabled = false;
        m_renderer.enabled = false;
       
        orbParticles.Stop();
        brokenOrbParticles.Play();

        DOVirtual.Float(1f, 0f, 0.2f, SetOrbFresnel);
    }
    private void SetOrbFresnel(float x)
    {
        dashResetMaterial.SetFloat("_FresnelAmount", x);
    }
    private void EnableDashResetObject()
    {
        m_renderer.enabled = true;
        DOVirtual.Float(0, 1f, 0.5f, SetOrbFresnel);
        orbParticles.Play();
        orbCollider.enabled = true;
    }
}
