using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public FMODUnity.EventReference outOfTheCaveEvent;
    private FMOD.Studio.EventInstance outOfTheCaveInstance;

    public FMODUnity.EventReference endSceneEvent;
    private FMOD.Studio.EventInstance endSceneInstance;

    public GameObject Fox;
    public GameObject goldenApple;
    private float maxDistanceToEnd;

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

    public void BattleOutOfTheCave()
    {
        outOfTheCaveInstance.setParameterByName("OutOfTheCave_Paused", 1);
    }
    public void PauseOutOfTheCave()
    {
        outOfTheCaveInstance.setParameterByName("OutOfTheCave_Paused", 2);
        outOfTheCaveInstance.release();
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

    public void PlayEndSceneEvent()
    {
        endSceneInstance.getPlaybackState(out FMOD.Studio.PLAYBACK_STATE state);
        if (state != FMOD.Studio.PLAYBACK_STATE.PLAYING)
        {
            endSceneInstance = FMODUnity.RuntimeManager.CreateInstance(endSceneEvent);

            maxDistanceToEnd = Vector3.Distance(Fox.transform.position, goldenApple.transform.position);

            UpdateEndDistance();

            endSceneInstance.start();

            SnapshotManager.instance.SetEndSceneSnapshot();
        }
    }

    public void StopEndSceneEvent()
    {
        endSceneInstance.setParameterByName("PauseEndCutscene", 1);
        endSceneInstance.release();
    }

    public void UpdateEndDistance()
    {
        float distanceVector = Vector3.Distance(Fox.transform.position, goldenApple.transform.position);

        float normalizedDistance = distanceVector / maxDistanceToEnd;

        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("DistanceToEnd", normalizedDistance);
    }

    public void ResetEndDistance()
    {
        FMODUnity.RuntimeManager.StudioSystem.setParameterByName("DistanceToEnd", 1f);
    }

}
