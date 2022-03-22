using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActionChange : MonoBehaviour
{


    /*// Start is called before the first frame update
    void Awake()
    {
        InputSystem.onActionChange += ActionMap_actionTriggered;
    }

    private void ActionMap_actionTriggered(object obj, InputActionChange change)
    {
        var action = obj as InputAction;

        var actionMap = action?.actionMap ?? obj as InputActionMap;
        var actionAsset = actionMap?.asset ?? obj as InputActionAsset;
        GetPossibleSchemes(action);

        //var binding = inputAction.GetBindingForControl(inputAction.activeControl).Value;
        //Debug.Log(binding.groups);
    }
    private static void GetPossibleSchemes(InputAction action)
    {
        char[] separator = new char[] { ';' };
        foreach (var binding in action.bindings)
        {
            if (InputControlPath.Matches(binding.effectivePath, action.activeControl))
            {
                // A binding can be assigned to multiple InputSchemes - loop over them
                foreach (string group in binding.groups.Split(separator, StringSplitOptions.RemoveEmptyEntries))
                {
                    Debug.Log(group);
                }
            }
        }
    }*/
}
