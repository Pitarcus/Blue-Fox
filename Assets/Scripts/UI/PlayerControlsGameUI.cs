using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine;
using DG.Tweening;

public class PlayerControlsGameUI : MonoBehaviour
{
    public Sprite jumpKeyboard;
    public Sprite jumpGamepad;
    private Sprite currentJump;

    [Space]
    public Sprite dashKeyboard;
    public Sprite dashGamepad;
    private Sprite currentDash;

    [Space]
    public Sprite confirmKeyboard;
    public Sprite confirmGamepad;
    private Sprite currentConfirm;

    [Space]
    public Image imageOnTopOfPlayer;
    public GameObject UICanvas;

    [Space] [Tooltip("Leave unassigned if dash button is not going to appear in animation")]
    public DashTriggerTutorial dashTut;


    private void Start()
    {
        UICanvas.SetActive(false);

        currentJump = jumpKeyboard;
        currentDash = dashKeyboard;
        currentConfirm = confirmKeyboard;

        if(dashTut != null)
            dashTut.dashTutorialComplete.AddListener(TweenCanvasOut);
    }
    private void Update()
    {
        transform.LookAt(2 * transform.position - Camera.main.transform.position);
    }
    private void OnEnable()
    {
        InputUser.onChange += InputUser_onChange;
    }
    private void OnDisable()
    {
        InputUser.onChange -= InputUser_onChange;
    }

    // Manage UI type
    private void InputUser_onChange(InputUser arg1, InputUserChange arg2, InputDevice arg3)
    {
        if (arg2 == InputUserChange.ControlsChanged)
        {
            Debug.Log("Device Change");
            if (arg1.controlScheme.Value.name == "Gamepad")
            {
                currentJump = jumpGamepad;
                currentDash = dashGamepad;
                currentConfirm = confirmGamepad;
            }
            else
            {
                currentJump = jumpKeyboard;
                currentDash = dashKeyboard;
                currentConfirm = confirmKeyboard;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("JumpUITrigger"))
        {
            imageOnTopOfPlayer.sprite = currentJump;
            TweenCanvasIn();
        }
        else if (other.CompareTag("DashUITrigger"))
        {
            imageOnTopOfPlayer.sprite = currentDash;
            TweenCanvasIn();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (dashTut == null)
            TweenCanvasOut();
    }
    private void TweenCanvasIn()
    {
        UICanvas.SetActive(true);
        DOVirtual.Float(0f, 1f, 0.3f, SetCanvasScale).SetEase(Ease.InFlash).SetUpdate(true);
    }

    private void TweenCanvasOut()
    {
        DOVirtual.Float(1f, 0f, 0.3f, SetCanvasScale).SetEase(Ease.InFlash).OnComplete(() => UICanvas.SetActive(false));
    }

    private void SetCanvasScale(float x)
    {
        UICanvas.transform.localScale = new Vector3(x, x, x);
    }
}
