using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticMovement : MonoBehaviour
{

    public FoxMovement playerMovement;
    public AnimateBars bars;
    public Transform targetPosition;

    private bool move = false;
    private Vector2 movementVector;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            StartAutomaticMovement();
    }

    public void StartAutomaticMovement() 
    {
        bars.PlayEnterBars();
        playerMovement.OnDisable();
        move = true;
        movementVector = Vector3.zero;

        playerMovement.forward = transform.forward;
        playerMovement.right = transform.right;
    }

    // Update is called once per frame
    void Update()
    {
        if (move) 
        {
            if (playerMovement.transform.position.x < targetPosition.position.x - 1)
                movementVector.x = 1;
            else if (playerMovement.transform.position.x > targetPosition.position.x + 1)
                movementVector.x = -1;
            else
                movementVector.x = 0;

            if (playerMovement.transform.position.z < targetPosition.position.z)
                movementVector.y = 1;
            else
                StopAutomaticMovement();

            if (move)
                playerMovement.inputVector = movementVector;
        }
    }

    public void StopAutomaticMovement()
    {
        move = false;
        playerMovement.inputVector = Vector2.zero;
        playerMovement.OnEnable();
        bars.PlayExitBars();
        Invoke("RecalculateCameraVectors", 1.2f);
    }

    private void RecalculateCameraVectors() 
    {
        Debug.Log("RECALCULANDO VECTORES");
        playerMovement.CalculateForwardVectors();
    }
}
