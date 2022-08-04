using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITransitionAudio : MonoBehaviour
{

    public FMODUnity.EventReference transitionIN;
    public FMODUnity.EventReference transitionOUT;

    public void playTransitionIn()
    {
        FMODUnity.RuntimeManager.PlayOneShot(transitionIN);
    }   
    public void playTransitionOut()
    {
        FMODUnity.RuntimeManager.PlayOneShot(transitionOUT);
    }
}
