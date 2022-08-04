using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class ThrowingPlatformLine : MonoBehaviour
{

    public Transform targetPosition;

    public LineRenderer lineRenderer;
    private Vector3[] vertexPositions = new Vector3[4];
    private float angle;

    private void Awake()
    {
        vertexPositions = new Vector3[4];
    }
    void Start()
    {
        UpdateVertexPositions();
        CalculateAngle();
    }

    void Update()
    {
        if(!Application.isPlaying)
        {
            UpdateVertexPositions();
        }
    }
    
    void CalculateAngle()
    {
        Vector3 targetDirection = targetPosition.position - transform.position;
        angle = Vector3.Angle(targetDirection, transform.forward);
    }

    void UpdateVertexPositions()
    {
        CalculateAngle();
        if (angle < 45 || angle > 135)
        {
            vertexPositions[0] = new Vector3(transform.position.x, transform.position.y - 4, transform.position.z);
            vertexPositions[1] = new Vector3(targetPosition.position.x, targetPosition.position.y - 4, targetPosition.position.z);
            vertexPositions[2] = new Vector3(targetPosition.position.x, targetPosition.position.y - 7, targetPosition.position.z);
            vertexPositions[3] = new Vector3(transform.position.x, transform.position.y - 7, transform.position.z);
        }
        else
        {
            vertexPositions[0] = new Vector3(transform.position.x, transform.position.y - 5, transform.position.z - 2);
            vertexPositions[1] = new Vector3(targetPosition.position.x, targetPosition.position.y, targetPosition.position.z - 2);
            vertexPositions[2] = new Vector3(targetPosition.position.x, targetPosition.position.y, targetPosition.position.z + 2);
            vertexPositions[3] = new Vector3(transform.position.x, transform.position.y - 5, transform.position.z + 2);
        }

        lineRenderer.SetPositions(vertexPositions);
    }
}
