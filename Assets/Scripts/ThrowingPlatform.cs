using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ThrowingPlatform : MonoBehaviour
{

    // Parameters
    public Transform targetPosition;
    public float duration = 2f;
    public float jumpVelocityMultiplier;
    public Ease easeType;

    // Platform members
    private Vector3 originalPosition;
    private Vector3 movingDirection;
    private bool onOrigin = true;
    public float jumpVelocity;  // ONLY PUBLIC FOR TESTING PURPOSES

    // References
    private GameObject player;
    private Rigidbody playerRb;

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.position;
        movingDirection = targetPosition.position - transform.position;
        movingDirection.Normalize();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (onOrigin)
        {
            // Create the sequence of the movement of the platform
            Sequence movementSequence = DOTween.Sequence();

            movementSequence.Append(transform.DOMove(targetPosition.position, duration, false)).SetEase(easeType);
            movementSequence.AppendInterval(1f);
            movementSequence.Append(transform.DOMove(originalPosition, duration + 1f, false)).OnComplete(OnOrigin);

            // Create the sequence of the values for the inertia of the player jump
            Sequence jumpSequence = DOTween.Sequence();
            jumpSequence.Append(DOVirtual.Float(0f, 1f, duration / 3, ChangeJumpVelocity));
            jumpSequence.AppendInterval(duration / 3);
            jumpSequence.Append(DOVirtual.Float(1f, 0f, duration / 3, ChangeJumpVelocity));
            //DOVirtual.Float(0f, 0.9f, duration + 0.2f, ChangeJumpVelocity).SetEase(Ease.InOutElastic).OnComplete(ResetJumpVelocity);

            onOrigin = false;
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
    private void OnTriggerEnter(Collider other)
    {
        if (player == null) 
        {
            player = other.gameObject;
            playerRb = player.GetComponent<Rigidbody>();
        }
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

