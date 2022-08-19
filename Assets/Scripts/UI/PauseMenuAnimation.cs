using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseMenuAnimation : MonoBehaviour
{
    [Header("Assign in Editor")]
    private FoxMovement foxMovement;
    public PlayerInputMixer playerInputMixer;
    public GameObject pauseMenuGameObject;
    public GameObject pauseButtons;
    public Image backgroundColorImage;
    public GameObject firstOptionsButtonsSelected;
    public Menu menuScript;
    public SceneJump sceneJumpScript;
    public FMODUnity.EventReference pauseSnapshot;
    public GameObject confirmIndicator;

    [Header("Pause Background Parameters")]
    public float startingAlphaValue = 0.4f;
    public float finalAlphaValue = 0.7f;

    private bool showingPauseMenu = false;
    private TimeManager timeManagerInstance;
    private FMOD.Studio.EventInstance pauseSnapshsotInstance;

    private void Awake()
    {
        Color auxColor = backgroundColorImage.color;
        auxColor.a = startingAlphaValue;
        backgroundColorImage.color = auxColor;

        foxMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<FoxMovement>();
        
    }
    private void Start()
    {
        pauseSnapshsotInstance = FMODUnity.RuntimeManager.CreateInstance(pauseSnapshot);
        timeManagerInstance = TimeManager.instance;
        foxMovement.input.CharacterControls.PauseGame.performed += ToggleMenu;
        confirmIndicator.SetActive(false);
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
        pauseSnapshsotInstance.start();
        DOVirtual.Float(0f, 1f, 0.8f, ChangePauseSnapshotParameter).SetUpdate(true);

        foxMovement.input.UI.Enable();
        playerInputMixer.SwitchInputToUI();

        timeManagerInstance.SmoothStopTime(0, false, 0);

        pauseMenuGameObject.SetActive(true);
        pauseButtons.SetActive(true);

        backgroundColorImage.DOFade(finalAlphaValue, 0.3f).SetUpdate(true);

        EventSystem.current.SetSelectedGameObject(firstOptionsButtonsSelected);

        confirmIndicator.SetActive(true);
    }

    public void HidePauseMenu()
    {
        DOVirtual.Float(1f, 0f, 0.8f, ChangePauseSnapshotParameter).SetUpdate(true)
            .OnComplete(() => pauseSnapshsotInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE));

        DisableSelectedButtons();

        foxMovement.input.UI.Disable();
        playerInputMixer.SwitchInputToMovement();

        pauseButtons.SetActive(false);

        backgroundColorImage.DOFade(startingAlphaValue, 0.2f).SetUpdate(true).OnComplete(() => pauseMenuGameObject.SetActive(false));

        Invoke("SetPauseMenuBoolFalse", 0.2f);

        confirmIndicator.SetActive(false);

        timeManagerInstance.SmoothPlayTime(0.2f);

        menuScript.CloseOptionsMenu();
    }

    public void DisableSelectedButtons()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    private void SetPauseMenuBoolFalse() 
    {
        showingPauseMenu = false;
    }

    private void ChangePauseSnapshotParameter(float x)
    {
        pauseSnapshsotInstance.setParameterByName("PauseMenuTransition", x);
    }

    public void ChangeScene(int scene)
    {
        sceneJumpScript.ChangeScene(scene);
        DOVirtual.Float(1f, 0f, 0.8f, ChangePauseSnapshotParameter).SetUpdate(true)
            .OnComplete(() => pauseSnapshsotInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE));
    }
}
