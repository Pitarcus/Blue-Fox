using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformsMusic : MonoBehaviour
{
    public FMODUnity.EventReference fallingPlatformsEvent;
    private FMOD.Studio.EventInstance fallinPlatformMusic;
    public DashTriggerTutorial dashTut;

    private void Start()
    {
        dashTut.dashTutorialComplete.AddListener(FallingPlatformsEnd);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fallinPlatformMusic = FMODUnity.RuntimeManager.CreateInstance(fallingPlatformsEvent);
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(fallinPlatformMusic, transform);
            fallinPlatformMusic.start();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            fallinPlatformMusic.setParameterByName("FallingPlatformPart", 1f);
        }
    }

    private void FallingPlatformsEnd() 
    {
        fallinPlatformMusic.setParameterByName("FallingPlatformPart", 2f);
        fallinPlatformMusic.release();
        GetComponent<Collider>().enabled = false;
    }
}
