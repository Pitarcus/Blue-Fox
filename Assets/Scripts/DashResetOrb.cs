using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DashResetOrb : MonoBehaviour
{
    private Material dashResetMaterial;
    private MeshRenderer renderer;
    private Collider orbCollider;
    private ParticleSystem orbParticles;
    private ParticleSystem brokenOrbParticles;

    void Start()
    {
        renderer = GetComponent<MeshRenderer>();
        dashResetMaterial = renderer.material;
        orbCollider = GetComponent<Collider>();

        orbParticles = GetComponentInChildren<ParticleSystem>();
        brokenOrbParticles = transform.GetChild(0).GetChild(0).GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider other)
    {
        AnimateDashReseter();
        Invoke("EnableDashResetObject", 1.5f);
    }
    private void AnimateDashReseter()
    {
        orbCollider.enabled = false;
        renderer.enabled = false;
       
        Debug.Log("Breaking orb...");
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
        renderer.enabled = true;
        DOVirtual.Float(0, 1f, 0.5f, SetOrbFresnel);
        orbParticles.Play();
        orbCollider.enabled = true;
    }
}
