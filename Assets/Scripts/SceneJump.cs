using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class SceneJump : MonoBehaviour
{
    PlayerInput input;
    private void Start()
    {
        input = new PlayerInput();
        input.CharacterControls.Enable();
        input.CharacterControls.ResetScene.performed += ctx => { ChangeScene(SceneManager.GetActiveScene().buildIndex); };
    }
    public void ChangeScene(int index)
    {
        // Changing Scene... 
        Debug.Log("Changing scene...");
        Time.timeScale = 1;
        StartCoroutine(LoadLevel(index));
    }
    IEnumerator LoadLevel(int levelIndex)
    {
        //transition.SetTrigger("Start");

        //yield return new WaitForSeconds(transitionTime);
        yield return new WaitForSeconds(0.2f);

        SceneManager.LoadScene(levelIndex);
    }
}
