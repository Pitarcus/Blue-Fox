using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopMusicAndPlay : MonoBehaviour
{
    [SerializeField]StudioEventEmitter backgroundMusicEventEmitter;
    [SerializeField] SceneJump sceneJump;

    public void StopMusicAndSceneJump(int i)
    {
        backgroundMusicEventEmitter.AllowFadeout = true;
        backgroundMusicEventEmitter.Stop();
        sceneJump.ChangeScene(i);
    }

    public void ExitApplication()
    {
        Application.Quit();
    }
}
