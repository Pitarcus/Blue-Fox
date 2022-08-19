using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodBagAudio : MonoBehaviour
{
    public FMODUnity.EventReference foodBagWingsEvent;

    FMOD.Studio.EventInstance foodBagWingsInstance;


    public void PlayBigWings()
    {
        foodBagWingsInstance = FMODUnity.RuntimeManager.CreateInstance(foodBagWingsEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(foodBagWingsInstance, transform);
        foodBagWingsInstance.setParameterByName("WingSize", 0f);
        foodBagWingsInstance.start();
        foodBagWingsInstance.release();
    }

    public void PlaySmallWings()
    {
        foodBagWingsInstance = FMODUnity.RuntimeManager.CreateInstance(foodBagWingsEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(foodBagWingsInstance, transform);
        foodBagWingsInstance.setParameterByName("WingSize", 1f);
        foodBagWingsInstance.start();
        foodBagWingsInstance.release();
    }

}
