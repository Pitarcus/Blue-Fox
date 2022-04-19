using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointUpdate : MonoBehaviour
{
    public Transform spawn;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            spawn.position = transform.position;
        }
    }
}
