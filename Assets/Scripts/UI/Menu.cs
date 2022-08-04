using UnityEngine.EventSystems;
using UnityEngine;
using DG.Tweening;
using Cinemachine;

public class Menu : MonoBehaviour
{
    public RectTransform mainButtons;
    public float hiddenXPosition = -160;
    public float shownXPosition = 40;
    public GameObject firstMainButtonsSelected;

    public RectTransform optionsButtons;
    public GameObject firstOptionsButtonsSelected;

    public CinemachineVirtualCamera mainCamera;
    public CinemachineVirtualCamera optionsCamera;

    public bool useDifferentCameras = true;

    public void OpenOptionsMenu()
    {
        Sequence seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(mainButtons.DOAnchorPosX(hiddenXPosition, 0.4f));
        seq.Append(optionsButtons.DOAnchorPosX(0, 0.4f));

        if (useDifferentCameras)
        {
            mainCamera.Priority = 5;
            optionsCamera.Priority = 10;
        }

        EventSystem.current.SetSelectedGameObject(firstOptionsButtonsSelected); 
    }

    public void CloseOptionsMenu()
    {
        Sequence seq = DOTween.Sequence().SetUpdate(true);
        seq.Append(optionsButtons.DOAnchorPosX(-800, 0.4f));
        seq.Append(mainButtons.DOAnchorPosX(shownXPosition, 0.4f));

        if (useDifferentCameras)
        {
            mainCamera.Priority = 10;
            optionsCamera.Priority = 0;
        }

        EventSystem.current.SetSelectedGameObject(firstMainButtonsSelected);
    }
}
