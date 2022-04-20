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
    FMOD.Studio.EventInstance footSteps;
    
    FMOD.Studio.PARAMETER_ID materialParameterId;

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

    private void OnDestroy()
    {
        footSteps.release();
    }

    public void SetMaterial(GroundTypes material)
    {
        footSteps.setParameterByID(materialParameterId, (float)material);
    }

    public void PlayFootstep() 
    {
        footSteps.start();
    }
}
