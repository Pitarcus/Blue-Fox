using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;
//using UnityEngine.InputSystem;

public class SceneJump : MonoBehaviour
{
    //PlayerInput input;
    [SerializeField]
    private Animator transition;
    [SerializeField]
    private float transitionTime;

    private void Start()
    {
       // input = new PlayerInput();
        //input.CharacterControls.Enable();
    }
    public void ChangeScene(int index)
    {
        // Debug.Log("Changing scene...");

        Time.timeScale = 1;
        StartCoroutine(LoadLevel(index));
    }
    IEnumerator LoadLevel(int levelIndex)
    {
        transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);
       
        SceneManager.LoadScene(levelIndex);
    }
}
