using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThrowingPlatform : MonoBehaviour
{

    // Parameters
    public Transform targetPosition;
    public Transform mesh;
    public float duration = 2f;
    public float jumpVelocityMultiplier;

    public bool fallingPLatform;
    public Ease easeType;
    public bool resetPosition = true;
    
    // Platform members
    private Vector3 originalPosition;
    private Vector3 movingDirection;
    private bool onOrigin = true;
    private float jumpVelocity;  // ONLY PUBLIC FOR TESTING PURPOSES

    // References
    private GameObject player;
    private Rigidbody playerRb;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        if (!fallingPLatform)
        {
            originalPosition = transform.position;
            movingDirection = targetPosition.position - transform.position;
            movingDirection.Normalize();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (onOrigin)
        {
            if (fallingPLatform)
                FallingPlatform();
            else
                ThrowPlatform();
        }
    }

    void ChangeJumpVelocity(float x) 
    {
        jumpVelocity = x;
    }

    void OnOrigin() 
    {
        onOrigin = true;
    }

    private void ThrowPlatform() 
    {
        // Create the sequence of the movement of the platform
        Sequence movementSequence = DOTween.Sequence();

        movementSequence.Append(transform.DOShakePosition(duration / 5));   // May change for a child mesh
        movementSequence.Join(transform.DOMove(targetPosition.position, duration, false).SetEase(easeType));

        if (resetPosition)
        {
            movementSequence.AppendInterval(1f);
            movementSequence.Append(transform.DOMove(originalPosition, duration + 1f, false)).OnComplete(OnOrigin);
        }

        // Create the sequence of the values for the inertia of the player jump
        Sequence jumpSequence = DOTween.Sequence();
        jumpSequence.Append(DOVirtual.Float(0f, 1f, duration, ChangeJumpVelocity).SetEase(easeType));
        jumpSequence.AppendInterval(0.1f);
        jumpSequence.Append(DOVirtual.Float(1f, 0f, 0.3f, ChangeJumpVelocity));
        //DOVirtual.Float(0f, 0.9f, duration + 0.2f, ChangeJumpVelocity).SetEase(Ease.InOutElastic).OnComplete(ResetJumpVelocity);

        onOrigin = false;
    }
    private void FallingPlatform() 
    {
        // Create the sequence of the movement of the platform
        Sequence movementSequence = DOTween.Sequence();

        movementSequence.Append(mesh.DOShakePosition(0.2f, 1, 30, 10, false, true));
        movementSequence.AppendInterval(0.5f);
        movementSequence.Join(transform.DOMove(transform.position - new Vector3(0, 200), 3f, false)).SetEase(Ease.InQuart);

        if (resetPosition)
        {
            movementSequence.AppendInterval(1f);
            movementSequence.Append(transform.DOMove(originalPosition, duration + 1f, false)).OnComplete(OnOrigin);
        }
        onOrigin = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (player == null) 
        {
            player = other.gameObject;
            playerRb = player.GetComponent<Rigidbody>();
        }
        if (!fallingPLatform)
            player.transform.parent = transform;
    }

    private void OnTriggerExit(Collider other)
    {
        player.transform.parent = null;
        playerRb.velocity += movingDirection * jumpVelocity * jumpVelocityMultiplier;
    }
    /*
    IEnumerator StartThrowing() 
    {
        for (int i = 0; i < 10; i++)
            platformRigidbody.AddForce(velocityMultiplier * movingDirection);

        yield return new WaitForSeconds(.3f);

        while (transform.position  != targetPosition.position - movingDirection * 2)
        {
            platformRigidbody.velocity = velocityMultiplier * movingDirection;
        }


    }

    void StopThrowing() 
    {

    }*/
}

