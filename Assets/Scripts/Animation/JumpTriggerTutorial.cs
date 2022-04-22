using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JumpTriggerTutorial : MonoBehaviour
{
    private PlayerInput playerInput;
    private FoxMovement playerMovement;
    private TimeManager timeManagerInstanceReference;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Please JUMP");

            playerMovement = other.gameObject.GetComponent<FoxMovement>();
            playerInput = playerMovement.input;

            playerInput.CharacterControls.Jump.performed += OnJumpPressed;
            playerInput.CharacterControls.Jump.canceled += OnJumpReleased;

            timeManagerInstanceReference =TimeManager.instance;
            timeManagerInstanceReference.SmoothStopTime(0.1f, false, 0f);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DisableListeners();
        }
    }

    private void OnJumpPressed(InputAction.CallbackContext context) 
    {
        Debug.Log("KEEP JUMPING");
        timeManagerInstanceReference.SmoothPlayTime(0.1f);
    }
    private void OnJumpReleased(InputAction.CallbackContext context) 
    {
        playerMovement.isJumping = true;
        Debug.Log(playerMovement.isJumping);
        timeManagerInstanceReference.SmoothStopTime(0f, false, 0f);
    }

    private void DisableListeners() 
    {
        playerInput.CharacterControls.Jump.performed -= OnJumpPressed;
        playerInput.CharacterControls.Jump.canceled -= OnJumpReleased;

    }
}
