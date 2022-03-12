using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Berries : MonoBehaviour
{
    private FoxFood foxFood;
    private FoxMovement foxMovement;
    static public int berryValue = 10;
    static public float maxTime = 3f;

    private PlayerInput input;
    private bool playerInRange = false;
    private float currentTime = 0f;
    private bool foodButtonDown = false;
    private bool assigned = false;

    // Shake properties
    public float shakeAmountY = 10f;
    public float shakeAmountZ = 10f;
    public float shakeFrequency = 10f;
    public GameObject berriesGameObject;
    private Vector3 originPosition;
    //private Quaternion originRotation;
    
    private void Start()
    {
        originPosition = berriesGameObject.transform.position;
        //originRotation = berriesGameObject.transform.rotation;
    }
    private void Update()
    {
        if (playerInRange && foodButtonDown)
        {
            currentTime += Time.deltaTime;
           
            ShakeBerries();

            if (currentTime > maxTime)
            {
                foxFood.IncreaseFoodAmount(berryValue);
                foxMovement.input.CharacterControls.GetFood.performed -= ctx => PressingFoodButton();
                foxMovement.input.CharacterControls.GetFood.canceled -= ctx => FoodButtonReleased();
                berriesGameObject.SetActive(false);
                this.enabled = false;
            }
        }
        else if (currentTime > 0) 
        {
            currentTime -= Time.deltaTime;
        }
    }
    private void ShakeBerries() 
    {
        berriesGameObject.transform.position = originPosition + new Vector3(0,
                Mathf.PerlinNoise(Time.time * shakeFrequency, 5) * shakeAmountY - 0.5f,
                Mathf.PerlinNoise(Time.time * shakeFrequency, 1) * shakeAmountZ - 1f
                );
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if (!assigned)
            {
                foxFood = other.GetComponent<FoxFood>();
                foxMovement = other.GetComponent<FoxMovement>();
                foxMovement.input.CharacterControls.GetFood.performed += ctx => PressingFoodButton();
                foxMovement.input.CharacterControls.GetFood.canceled += ctx => FoodButtonReleased();
            }
            playerInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player NOT in range...");
            playerInRange = false;
        }
    }
    private void PressingFoodButton ()
    {
        if (playerInRange)
        {
            foxMovement.canMove = false;
            Debug.Log("Food Button PRESSED while close");
            foodButtonDown = true;
        }
    }
    private void FoodButtonReleased ()
    {
        if (playerInRange)
        {
            foxMovement.canMove = true;
            Debug.Log("Food Button RELEASED while close");
            foodButtonDown = false;
        }
    }
}
