using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MeleeEnemyFX : MonoBehaviour
{
    public FMODUnity.EventReference walkingEvent;
    public FMODUnity.EventReference attackEvent;
    public FMODUnity.EventReference weakEvent;
    public VisualEffect attackVFX;

    private FMOD.Studio.PARAMETER_ID recoveryParameterId;
    private static FMOD.Studio.EventInstance weak;

    private void Start()
    {
        weak = FMODUnity.RuntimeManager.CreateInstance(weakEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(weak, transform);

        FMOD.Studio.EventDescription weakEventDescription;
        weak.getDescription(out weakEventDescription);

        FMOD.Studio.PARAMETER_DESCRIPTION recoveryParameterDescription;
        weakEventDescription.getParameterDescriptionByName("Recovery", out recoveryParameterDescription);

        recoveryParameterId = recoveryParameterDescription.id;
    }
    public void PlayWalking()
    {
        FMOD.Studio.EventInstance walking = FMODUnity.RuntimeManager.CreateInstance(walkingEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(walking, transform);
        walking.start();
        walking.release();
    }
    public void PlayAttack()
    {
        weak.setParameterByID(recoveryParameterId, 0f);
        FMOD.Studio.EventInstance attack = FMODUnity.RuntimeManager.CreateInstance(attackEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(attack, transform);
        attack.start();
        attack.release();
    }
    public void PlayWeak()
    {
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(weak, transform);
        weak.start();
    }
    public void PlayRecovery()
    { 
        weak.setParameterByID(recoveryParameterId, 1f);
    }

    public void PlayDeath()
    {
        weak.setParameterByID(recoveryParameterId, 0f);
        weak.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        weak.release();
        // Other sound of death
    }

    public void PlayAttackVFX()
    {
        attackVFX.Play();
    }

    public void StopAll()
    {
        weak.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
