using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnapshotManager : MonoBehaviour
{
    public FMODUnity.EventReference caveSnapshot;
    public FMODUnity.EventReference outDoorsSnapshot;

    private static FMOD.Studio.EventInstance snapshot;

    private void Awake()
    {
        SceneManager.sceneLoaded += SetSnapShot;
        SceneManager.sceneUnloaded += ResetSnapShot;
    }

    void SetSnapShot(Scene scene, LoadSceneMode mode)
    {
        
        if (SceneManager.GetActiveScene().name == "Cave") 
        {
            snapshot = FMODUnity.RuntimeManager.CreateInstance(caveSnapshot);
        }   
        else 
        {
            snapshot = FMODUnity.RuntimeManager.CreateInstance(outDoorsSnapshot);
        }
        snapshot.start();
    }

    private void ResetSnapShot(Scene scene)
    {
        SceneManager.sceneLoaded -= SetSnapShot;
        snapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        snapshot.release();
    }

}
