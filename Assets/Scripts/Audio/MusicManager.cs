using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public FMODUnity.EventReference outOfTheCaveEvent;
    private FMOD.Studio.EventInstance outOfTheCaveInstance;

    public static MusicManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        outOfTheCaveInstance = FMODUnity.RuntimeManager.CreateInstance(outOfTheCaveEvent);
    }

    public void StartPlayingOutOfTheCave()
    {
        outOfTheCaveInstance.start();
    }

    public void PlayOutOfTheCaveBody()
    {
        outOfTheCaveInstance.setParameterByName("OutOfTheCaveBody", 1);
    }

    public void PauseOutOfTheCave()
    {
        outOfTheCaveInstance.setParameterByName("OutOfTheCave_Paused", 1);
    }

    public void ResumeOutOfTheCave()
    {
        outOfTheCaveInstance.setParameterByName("OutOfTheCave_Paused", 0);
    }
    public void OutOfTheCaveAddMistery()
    {
        outOfTheCaveInstance.setParameterByName("Misterious", 1);
    }
    public void OutOfTheCaveRemoveMistery()
    {
        outOfTheCaveInstance.setParameterByName("Misterious", 0);
    }
}
