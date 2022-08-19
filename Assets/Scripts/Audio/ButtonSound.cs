using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    [SerializeField]
    private FMODUnity.EventReference uiMove;
    [SerializeField]
    private FMODUnity.EventReference uiSelect;

    public void PlayUIMove()
    {
        FMODUnity.RuntimeManager.PlayOneShot(uiMove);
    }
    public void PlayUISelect()
    {
        FMODUnity.RuntimeManager.PlayOneShot(uiSelect);
    }
}
