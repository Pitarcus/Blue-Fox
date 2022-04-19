using System.Collections;
using UnityEngine;
using DG.Tweening;

public class TimeManager : MonoBehaviour
{
    public static TimeManager instance;

    private void Awake()
    {
        instance = this;
    }
    public void PauseTime(float pauseDuration) 
    {
        Time.timeScale = 0;
        StartCoroutine(PlayTime(pauseDuration));
    }
    private IEnumerator PlayTime(float delay) 
    {
        yield return new WaitForSecondsRealtime(delay);
        Time.timeScale = 1;
    }
    public void SmoothStopTime(float stopTime, bool replay, float playTime) 
    {
        if (replay)
            DOVirtual.Float(1f, 0f, stopTime, ChangeTimeScale).SetEase(Ease.Linear).SetUpdate(true).OnComplete(() => SmoothPlayTime(playTime));
        else
            DOVirtual.Float(1f, 0f, stopTime, ChangeTimeScale).SetEase(Ease.Linear).SetUpdate(true);
    }

    public void SmoothPlayTime(float playTime) 
    {
        DOVirtual.Float(0, 1f, playTime, ChangeTimeScale).SetEase(Ease.Linear).SetUpdate(true);
    }

    private void ChangeTimeScale(float x)
    {
        Time.timeScale = x;
    }
}
