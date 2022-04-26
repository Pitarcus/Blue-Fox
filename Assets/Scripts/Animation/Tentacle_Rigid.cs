using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle_Rigid : MonoBehaviour
{
    public int lenght;
    public LineRenderer lineRenderer;
    public Vector3[] segmentPoses;
    private Vector3[] segmentVelocity;

    public Transform targetDirection;
    public float targetDistance;
    public float smoothSpeed;

    private void Start()
    {
        lineRenderer.positionCount = lenght;
        segmentPoses = new Vector3[lenght];
        segmentVelocity = new Vector3[lenght];

        // Start position relative to the first segment
        segmentPoses[0] = targetDirection.position;
        for (int i = 1; i < segmentPoses.Length; i++)
        {
            segmentPoses[i] = segmentPoses[i - 1] - targetDirection.up * targetDistance;
        }
    }

    private void Update()
    {
        segmentPoses[0] = targetDirection.position;

        for (int i = 1; i < segmentPoses.Length; i++)
        {
            Vector3 targetPos = segmentPoses[i - 1] + (segmentPoses[i] - segmentPoses[i - 1]).normalized * targetDistance;
            segmentPoses[i] = Vector3.SmoothDamp(segmentPoses[i], targetPos, ref segmentVelocity[i], smoothSpeed);
        }

        lineRenderer.SetPositions(segmentPoses);
    }
}
