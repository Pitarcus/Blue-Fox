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

    private bool chanching = false;

    private void Start()
    {
        chanching = false;
    }
    public void ChangeScene(int index)
    {
        if (!chanching)
        {
            chanching = true;
            Time.timeScale = 1;
            transition.SetTrigger("Start");
            StartCoroutine(LoadLevel(index));
        }
    }
    IEnumerator LoadLevel(int levelIndex)
    {
        yield return new WaitForSeconds(transitionTime);

        Scene activeScene = SceneManager.GetActiveScene();

        //SceneManager.LoadScene(levelIndex);
        AsyncOperation asyncLoadLevel = SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Single);

        // Wait until the level finishes loading
        while (!asyncLoadLevel.isDone)
            yield return null;
        // Wait a frame so every Awake and Start method is called
        yield return new WaitForEndOfFrame();
    }
}
