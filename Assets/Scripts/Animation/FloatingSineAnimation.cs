using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingSineAnimation : MonoBehaviour
{
    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float upAmplitude;
    [SerializeField]
    private float upPeriod;

    private Vector3 rotation;
    private Vector3 newPosition;

    private void Start()
    {
        rotation = new Vector3(0, turnSpeed, 0);
       
    }
    void Update()
    {
        rotation = new Vector3(0, turnSpeed, 0);
        transform.Rotate(rotation);
        newPosition = transform.position;
        upPeriod = Mathf.Clamp(upPeriod, 0.001f, 10);
        newPosition.y += upAmplitude * (2*Mathf.PI/ upPeriod * Mathf.Sin(Time.time)) * Time.deltaTime;
        transform.position = newPosition;
    }
}
