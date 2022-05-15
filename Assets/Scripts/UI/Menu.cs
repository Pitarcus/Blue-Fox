using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class Menu : MonoBehaviour
{
    public RectTransform mainButtons;
    public RectTransform optionsButtons;

    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera optionsCamera;

    public void OpenOptionsMenu()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(mainButtons.DOAnchorPosX(-160, 0.4f));
        seq.Append(optionsButtons.DOAnchorPosX(0, 0.4f));

        mainCamera.Priority = 0;
        optionsCamera.Priority = 10;
    }

    public void CloseOptionsMenu()
    {
        Sequence seq = DOTween.Sequence();
        seq.Append(optionsButtons.DOAnchorPosX(-800, 0.4f));
        seq.Append(mainButtons.DOAnchorPosX(40, 0.4f));

        mainCamera.Priority = 10;
        optionsCamera.Priority = 0;
    }
}
