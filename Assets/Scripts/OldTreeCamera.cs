using Cinemachine;
using UnityEngine;

public class OldTreeCamera : MonoBehaviour
{
    public CinemachineVirtualCamera camera;
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
            camera.Priority = 10;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            camera.Priority = 0;
    }
}
