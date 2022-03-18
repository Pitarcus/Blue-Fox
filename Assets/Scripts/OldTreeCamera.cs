using Cinemachine;
using UnityEngine;

public class OldTreeCamera : MonoBehaviour
{
    public CinemachineVirtualCamera treeCamera;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            treeCamera.Priority = 10;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            treeCamera.Priority = 0;
    }
}
