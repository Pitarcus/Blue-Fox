using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneJumpByTrigger : MonoBehaviour
{
    public int sceneToJump;
    public SceneJump sceneJump;

    private void OnTriggerEnter(Collider other)
    {
        sceneJump.ChangeScene(sceneToJump);
    }
}
