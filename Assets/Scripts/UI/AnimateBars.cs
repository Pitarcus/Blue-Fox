using DG.Tweening;
using UnityEngine;

public class AnimateBars : MonoBehaviour
{
    public float animationDuration = 3;

    public RectTransform topBar;
    public RectTransform botBar;

    public float topStartY;
    public float botStartY;

    private void Start()
    {
        topStartY = topBar.anchoredPosition.y;
        botStartY = botBar.anchoredPosition.y;

        topBar.anchoredPosition = new Vector3(0, -topStartY);
        botBar.anchoredPosition = new Vector3(0, -botStartY);
    }

    public void PlayEnterBars() 
    {
        topBar.DOAnchorPosY(topStartY, animationDuration);
        botBar.DOAnchorPosY(botStartY, animationDuration);
    }

    public void PlayExitBars()
    {
        topBar.DOAnchorPosY(-topStartY, animationDuration);
        botBar.DOAnchorPosY(-botStartY, animationDuration)
            .OnComplete(()=> gameObject.SetActive(false));
        
    }
}
