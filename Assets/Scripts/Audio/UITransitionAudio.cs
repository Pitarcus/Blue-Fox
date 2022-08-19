using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransitionAudio : MonoBehaviour
{

    public FMODUnity.EventReference transitionIN;
    public FMODUnity.EventReference transitionOUT;

    public void PlayTransitionIn()
    {
        FMODUnity.RuntimeManager.PlayOneShot(transitionIN);
    }   
    public void PlayTransitionOut()
    {
        FMODUnity.RuntimeManager.PlayOneShot(transitionOUT);
    }
}
