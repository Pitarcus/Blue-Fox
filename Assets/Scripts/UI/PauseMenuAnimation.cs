using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseMenuAnimation : MonoBehaviour
{
    private FoxMovement foxMovement;
    public PlayerInputMixer playerInputMixer;
    public GameObject pauseMenuGameObject;
    public GameObject pauseButtons;
    public Image backgroundColorImage;
    public float startingAlphaValue = 0.4f;
    public float finalAlphaValue = 0.7f;

    public GameObject firstOptionsButtonsSelected;

    private bool showingPauseMenu = false;
    private TimeManager timeManagerInstance;

    private void Awake()
    {
        Color auxColor = backgroundColorImage.color;
        auxColor.a = startingAlphaValue;
        backgroundColorImage.color = auxColor;

        foxMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<FoxMovement>();
        
    }
    private void Start()
    {
        timeManagerInstance = TimeManager.instance;
        foxMovement.input.CharacterControls.PauseGame.performed += ToggleMenu;
    }

    private void OnDisable()
    {
        foxMovement.input.CharacterControls.PauseGame.performed -= ToggleMenu;
    }

    public void ToggleMenu(InputAction.CallbackContext context)
    {
        if(!showingPauseMenu)
        {
            showingPauseMenu = true;
            ShowPauseMenu();
        }
        else
        {
            
            HidePauseMenu();
        }
    }
    private void ShowPauseMenu() 
    {
        foxMovement.input.UI.Enable();
        playerInputMixer.SwitchInputToUI();

        timeManagerInstance.SmoothStopTime(0, false, 0);

        pauseMenuGameObject.SetActive(true);
        pauseButtons.SetActive(true);

        backgroundColorImage.DOFade(finalAlphaValue, 0.3f).SetUpdate(true);

        EventSystem.current.SetSelectedGameObject(firstOptionsButtonsSelected);
    }

    public void HidePauseMenu()
    {
        DisableSelectedButtons();

        foxMovement.input.UI.Disable();
        playerInputMixer.SwitchInputToMovement();

        pauseButtons.SetActive(false);

        backgroundColorImage.DOFade(startingAlphaValue, 0.2f).SetUpdate(true).OnComplete(() => pauseMenuGameObject.SetActive(false));

        Invoke("SetPauseMenuBoolFalse", 0.2f);

        timeManagerInstance.SmoothPlayTime(0.2f);
    }

    public void DisableSelectedButtons()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void SetPauseMenuBoolFalse() 
    {
        showingPauseMenu = false;
    }
}
