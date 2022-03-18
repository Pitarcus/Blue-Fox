using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    private void Awake()
    {
        instance = this;
    }
    public void StopTime(float time) 
    {
        Time.timeScale = 0;
        StartCoroutine(PlayTime(time));
    }
    private IEnumerator PlayTime(float seconds) 
    {
        yield return new WaitForSecondsRealtime(seconds);
        Time.timeScale = 1;
    }
}
