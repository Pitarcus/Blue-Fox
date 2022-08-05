using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Strawberry : MonoBehaviour
{
    [SerializeField]
    private FoodCaught foodCaughtScript;
    [SerializeField]
    private FloatingSineAnimation floatingSineAnimation;
    
    public Collider trigger;
    [Tooltip("Z value is ignored as it can be modified if ther are more than one strawberries")]
    public Vector3 foodOffset;
    private float zOffset;
    public float smoothTime = 0.3f;

    private bool followingPlayer;
    private FoxMovement foxMovement;
    private FoxFood foxFoodScript;
    private FoxHealth foxHealth;
    private Transform foxTransform;
    private Vector3 velocity;

    private Vector3 originalPosition;


    private void Awake()
    {
        originalPosition = transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foxMovement = other.GetComponent<FoxMovement>();
            foxFoodScript = other.GetComponent<FoxFood>();
            foxHealth = other.GetComponent<FoxHealth>();

            foxHealth.playerDeath.AddListener(ResetStrawberry);
            foxHealth.playerRespawned.AddListener(ResetStrawberry);
            foxMovement.onTrueGround.AddListener(CompleteStrawberry);

            foxTransform = other.transform;

            // Calculate Position
            zOffset = foxFoodScript.potentialStrawberries * 5f;
            smoothTime += foxFoodScript.potentialStrawberries * 0.05f;
            foxFoodScript.potentialStrawberries++;

            followingPlayer = true;
            trigger.enabled = false;
            floatingSineAnimation.enabled = false;
        }
    }

    private void Update()
    {
        if(followingPlayer)
        {
            Vector3 localZOffset = foxTransform.forward * -zOffset;

            Vector3 newPosition = foxTransform.position + localZOffset;
            newPosition.x += foodOffset.x;
            newPosition.y += foodOffset.y;

            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
        }
    }
    void CompleteStrawberry()
    {
        followingPlayer = false;

        foxHealth.playerDeath.RemoveListener(ResetStrawberry);
        foxHealth.playerRespawned.RemoveListener(ResetStrawberry);
        foxMovement.onTrueGround.RemoveListener(CompleteStrawberry);

        foxFoodScript.potentialStrawberries--;

        foodCaughtScript.FoodCaughtFunction(foxTransform);
        this.enabled = false;
    }
    void ResetStrawberry()
    {
        foxFoodScript.potentialStrawberries = 0;
        followingPlayer = false;

        foxHealth.playerDeath.RemoveListener(ResetStrawberry);
        foxHealth.playerRespawned.RemoveListener(ResetStrawberry);
        foxMovement.onTrueGround.RemoveListener(CompleteStrawberry);

        transform.position = originalPosition;
        floatingSineAnimation.enabled = true;

        trigger.enabled = true;
    }
}
