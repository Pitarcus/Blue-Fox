using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundTypeUpdater : MonoBehaviour
{

    public FoxFootSteps.GroundTypes groundType;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player")) 
        {
            collision.gameObject.GetComponent<FoxFootSteps>().SetMaterial(groundType);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<FoxFootSteps>().SetMaterial(groundType);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<FoxFootSteps>().SetMaterial(FoxFootSteps.GroundTypes.Stone);
        }
    }
}
