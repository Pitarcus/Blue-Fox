using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenApple : MonoBehaviour
{
    public FMODUnity.EventReference appleIdleEvent;
    private FMOD.Studio.EventInstance appleIdleInstance;

    private void Start()
    {
        appleIdleInstance = FMODUnity.RuntimeManager.CreateInstance(appleIdleEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(appleIdleInstance, transform);
        appleIdleInstance.start();
    }

    public void EndApple()
    {
        appleIdleInstance.setParameterByName("Apple_End", 1);
        appleIdleInstance.release();
    }
}
