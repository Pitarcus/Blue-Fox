using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoxFootSteps : MonoBehaviour
{

    public enum GroundTypes 
    { 
        Snow,
        Stone,
        Hay
    };

    public FMODUnity.EventReference footStepEvent;

    // reference to the instance of the event
    private FMOD.Studio.EventInstance footSteps;

    private FMOD.Studio.PARAMETER_ID materialParameterId;
    private GroundTypes currentMaterial;

    // Start is called before the first frame update
    void Start()
    {
        footSteps = FMODUnity.RuntimeManager.CreateInstance(footStepEvent);

        FMOD.Studio.EventDescription footStepEventDescription;
        footSteps.getDescription(out footStepEventDescription);

        FMOD.Studio.PARAMETER_DESCRIPTION materialParameterDescription;
        footStepEventDescription.getParameterDescriptionByName("Material", out materialParameterDescription);

        materialParameterId = materialParameterDescription.id;
    }

    public void SetMaterial(GroundTypes material)
    {
        currentMaterial = material;
    }

    public void PlayFootstep() 
    {
        FMOD.Studio.EventInstance footstep = FMODUnity.RuntimeManager.CreateInstance(footStepEvent);
        footstep.setParameterByID(materialParameterId, (float)currentMaterial);
        footstep.start();
        footstep.release();
    }
}
