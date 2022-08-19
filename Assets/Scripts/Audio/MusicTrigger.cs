using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicTrigger : MonoBehaviour
{
    [SerializeField]
    private MusicManager musicManager;
    [SerializeField]
    private bool startOutOfTheCave = false;
    [SerializeField]
    private bool outOfTheCaveBody = false;
    [SerializeField]
    private bool addMistery = false;
    [SerializeField]
    private bool noMistery = false;
    [SerializeField]
    private bool stopOotC = false;

    private bool done = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!done)
        {
            done = true;
            if (startOutOfTheCave)
            {
                musicManager.StartPlayingOutOfTheCave();
            }
            else if (outOfTheCaveBody)
            {
                musicManager.PlayOutOfTheCaveBody();
            }
            else if (addMistery)
            {
                musicManager.OutOfTheCaveAddMistery();
            }
            else if (noMistery)
            {
                musicManager.OutOfTheCaveRemoveMistery();
            }
            else if (stopOotC)
                musicManager.PauseOutOfTheCave();
        }
    }
}
