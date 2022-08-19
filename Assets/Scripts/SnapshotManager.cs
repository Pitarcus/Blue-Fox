using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnapshotManager : MonoBehaviour
{
    public FMODUnity.EventReference caveSnapshot;
    public FMODUnity.EventReference battleSnapshot;
    public FMODUnity.EventReference endSceneSnapshot;

    private static FMOD.Studio.EventInstance snapshot;

    public static SnapshotManager instance;

    private void Awake()
    {
        SceneManager.sceneLoaded += SetSnapshot;
        SceneManager.sceneUnloaded += ResetSnapshot;

        instance = this;
    }

    void SetSnapshot(Scene scene, LoadSceneMode mode)
    {
        
        if (SceneManager.GetActiveScene().name == "Cave") 
        {
            snapshot = FMODUnity.RuntimeManager.CreateInstance(caveSnapshot);
            snapshot.start();
        }   
        else 
        {
            snapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    private void ResetSnapshot(Scene scene)
    {
        SceneManager.sceneLoaded -= SetSnapshot;
        SceneManager.sceneUnloaded -= ResetSnapshot;
        snapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        snapshot.release();
    }

    public void SetBattleSnapshot()
    {
        snapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        snapshot.release();
        snapshot = FMODUnity.RuntimeManager.CreateInstance(battleSnapshot);
        snapshot.start();
    }

    public void SetNormalSnapshot()
    {
        snapshot.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        snapshot.release();
    }

    public void SetEndSceneSnapshot() 
    {
        snapshot.release();
        snapshot = FMODUnity.RuntimeManager.CreateInstance(endSceneSnapshot);
        snapshot.start();
    }
}
