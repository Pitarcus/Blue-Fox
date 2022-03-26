using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using DG.Tweening;

public class Berries : MonoBehaviour
{
    [Header("Assign in editor")]
    public GameObject berriesGameObject;
    public CinemachineVirtualCamera zoomedInCamera;
    public GameObject UICanvas;
    public Image slider;
    public Image buttonImage;
    public Sprite controllerSprite;
    public Sprite keyboardSprite;

    [Header("Shake Parameters")]
    public float shakeAmountY = 10f;
    public float shakeAmountZ = 10f;
    public float shakeFrequency = 10f;

    [Header("Shake Parameters")]
    public float zoomScale;

    [Header("Berries Parameters")]
    // Berries properties
    static public int berryValue = 10;
    static public float maxTime = 3f;
    private Vector3 originPosition;

    private bool collected = false; // When collected, stop working
    private bool assigned = false; // Private variables referencing the player assigned

    private FoxFood foxFood;
    private FoxMovement foxMovement;
    private bool playerInRange = false;

    // Button stuff
    private bool foodButtonDown = false;
    private float currentTime = 0f;

    private PlayerInput input;

    private void Start()
    {
        originPosition = berriesGameObject.transform.position;
        UICanvas.SetActive(false);
    }
    // Stuf for changing UI depending on Device
    private void OnEnable()
    {
        InputUser.onChange += InputUser_onChange;
    }
    private void OnDisable()
    {
        InputUser.onChange -= InputUser_onChange;
    }

    private void InputUser_onChange(InputUser arg1, InputUserChange arg2, InputDevice arg3)
    {
        if (arg2 == InputUserChange.ControlsChanged)
        {
            Debug.Log("Device Change");
            if (arg1.controlScheme.Value.name == "Gamepad")
            {
                buttonImage.sprite = controllerSprite;
            }
            else
                buttonImage.sprite = keyboardSprite;
        }
    }

    private void Update()
    {
        if (!collected && playerInRange && foodButtonDown)  // Pressing button down
        {
            currentTime += Time.deltaTime;

            slider.fillAmount = currentTime / maxTime;

            ShakeBerries();

            zoomedInCamera.Priority = 10;

            if (currentTime > maxTime)  // Button pressed for long enough
            {
                zoomedInCamera.Priority = 0;

                foxFood.IncreaseFoodAmount(berryValue);

                input.CharacterControls.GetFood.performed -= ctx => PressingFoodButton();
                input.CharacterControls.GetFood.canceled -= ctx => FoodButtonReleased();

                berriesGameObject.SetActive(false);

                TweenCanvasOut();
                Invoke("DisableCanvas", 0.3f);

                collected = true;
                this.enabled = false;
            }
        }
        else if (currentTime > 0) 
        {
            slider.fillAmount = currentTime / maxTime;
            zoomedInCamera.Priority = 0;
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
        if (!collected && other.CompareTag("Player")) 
        {
            if (!assigned)
            {
                foxFood = other.GetComponent<FoxFood>();
                foxMovement = other.GetComponent<FoxMovement>();
                input = foxMovement.input;
                input.CharacterControls.GetFood.performed += ctx => PressingFoodButton();
                input.CharacterControls.GetFood.canceled += ctx => FoodButtonReleased();
            }
            playerInRange = true;

            UICanvas.SetActive(true);
            DOVirtual.Float(0f, 1f, 0.3f, SetCanvasScale).SetEase(Ease.InFlash);

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            foxMovement.canMove = true;
            TweenCanvasOut();
        }

    }
    private void TweenCanvasOut() 
    {
        DOVirtual.Float(1f, 0f, 0.3f, SetCanvasScale).SetEase(Ease.InFlash);
    }
    private void DisableCanvas()
    { 
        UICanvas.SetActive(false);
    }
    private void SetCanvasScale(float x)
    {
        UICanvas.transform.localScale = new Vector3(x, x, x);
    }

    private void PressingFoodButton ()
    {
        if (playerInRange)
        {
            foxMovement.canMove = false;
            foodButtonDown = true;
        }
    }
    private void FoodButtonReleased ()
    {
        if (playerInRange)
        {
            foxMovement.canMove = true;
            foodButtonDown = false;
        }
    }
    
}
