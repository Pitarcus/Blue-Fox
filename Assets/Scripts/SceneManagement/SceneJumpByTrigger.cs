using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneJumpByTrigger : MonoBehaviour
{
    public int sceneToJump;
    public SceneJump sceneJump;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            sceneJump.ChangeScene(sceneToJump);
            triggered = true;
        }
    }
}
