using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class PlayerInputMixer : MonoBehaviour
{
    public PlayerInput PlayerControls { get; private set; }

    public InputUI InputUI { get; private set; }

    public InputSystemUIInputModule uiInputModule;
    public UnityEngine.InputSystem.PlayerInput PlayerInput { get; private set; }

    public GameObject gameInputObject;

    void Awake()
    {
        PlayerControls = new PlayerInput(); // implements IInputActionCollection
        InputUI = new InputUI();

        PlayerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
        PlayerInput.defaultActionMap = PlayerControls.UI.Get().name;
        PlayerInput.actions = PlayerControls.asset;

        //var uiInputModule = gameInputObject.GetComponentInChildren<InputSystemUIInputModule>();
        uiInputModule.actionsAsset = InputUI.asset;

        PlayerInput.uiInputModule = uiInputModule;
    }

    void OnEnable()
    {
        // Creates users using your controls class & overrides the InputSystemUIInputModule with it.
        PlayerInput.ActivateInput();
    }

    // This is how you should the switch currently active action map
    public void SwitchInputToUI()
    {
        PlayerInput.currentActionMap = PlayerControls.UI.Get();
    }
    public void SwitchInputToMovement()
    {
        PlayerInput.currentActionMap = PlayerControls.CharacterControls.Get();
    }
}
