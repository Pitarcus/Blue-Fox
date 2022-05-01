using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DetatchPlayerFromCamera : MonoBehaviour
{
    public CinemachineVirtualCamera vc;
    public CinemachineVirtualCamera vcAlt;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            vc.Priority = 0;
            vcAlt.Priority = 10;
        }
    }
}
