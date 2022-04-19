using DG.Tweening;
using UnityEngine;

public class AnimateBars : MonoBehaviour
{
    public float animationDuration = 3;

    public RectTransform topBar;
    public RectTransform botBar;

    private void Start()
    {
        topBar.anchoredPosition = new Vector3(0, 50);
        botBar.anchoredPosition = new Vector3(0, -50);
    }

    public void PlayEnterBars() 
    {
        topBar.DOAnchorPosY(-30, animationDuration);
        botBar.DOAnchorPosY(30, animationDuration);
    }

    public void PlayExitBars()
    {
        topBar.DOAnchorPosY(30, animationDuration);
        botBar.DOAnchorPosY(-30, animationDuration);
    }
}
