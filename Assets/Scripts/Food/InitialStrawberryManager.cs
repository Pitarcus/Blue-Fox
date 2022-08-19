using UnityEngine.SceneManagement;
using UnityEngine;

public class InitialStrawberryManager : MonoBehaviour
{
    public static bool[] unlocked = new bool[10];
    public Strawberry strawberryScript;

    private void Awake()
    {
       
        SceneManager.sceneLoaded += CheckForStrawberry;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= CheckForStrawberry;
    }
    private void CheckForStrawberry(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Cave" || SceneManager.GetActiveScene().name == "BasicEnvironment")
        {
            if (unlocked[strawberryScript.strawberryIndex])
                Destroy(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void MarkStrawberry()
    {
        unlocked[strawberryScript.strawberryIndex] = true;
    }
}
