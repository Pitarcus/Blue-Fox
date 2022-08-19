using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine;
using UnityEngine.UI;

public class UIChangerFromDevice : MonoBehaviour
{
    public bool confirm = true;

    public Sprite confirmController;
    public Sprite confirmKeyboard;
    public Sprite backController;
    public Sprite backKeyboard;

    public Image imageToShow;
    public Sprite current;

    // Start is called before the first frame update
    void Start()
    {
        if (confirm)
        {
            current = confirmKeyboard;
        }
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
            if (arg1.controlScheme.Value.name == "Gamepad")
            {
                if (confirm)
                    current = confirmController;
                else
                    current = backController;
            }
            else
            {
                if (confirm)
                    current = confirmKeyboard;
                else
                    current = backKeyboard;
            }
            imageToShow.sprite = current;
        }
    }
}
