using Cinemachine;
using UnityEngine;

public class DollyTrigger : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera dollyCam;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            dollyCam.Priority = 11;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            dollyCam.Priority = 0;
        }
    }
}
