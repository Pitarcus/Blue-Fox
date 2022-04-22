using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DashTriggerTutorial : MonoBehaviour
{

    public float stopTimeDuration = 0.3f;
    private PlayerInput playerInput;
    private FoxMovement playerMovement;
    private TimeManager timeManagerInstanceReference;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement = other.gameObject.GetComponent<FoxMovement>();
            playerInput = playerMovement.input;

            playerInput.CharacterControls.Dash.performed += OnDashPressed;

            timeManagerInstanceReference = TimeManager.instance;
            timeManagerInstanceReference.SmoothStopTime(stopTimeDuration, false, 0f);
        }
    }
    private void OnDashPressed(InputAction.CallbackContext context)
    {
        timeManagerInstanceReference.SmoothPlayTime(0.05f);
        playerInput.CharacterControls.Dash.performed -= OnDashPressed;
        this.gameObject.SetActive(false);
    }
}
