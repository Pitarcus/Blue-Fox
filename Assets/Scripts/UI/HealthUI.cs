using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;

public class HealthUI : MonoBehaviour
{
    [Header("Assign in inspector")]
    public RectTransform heartsGroupPosition;
    public Image[] heartsImages;
    private CanvasGroup heartsGroupAlpha;
    public RectTransform heartsMask;

    [Space]
    [Header("Parameters")]
    public float transitionTime = 0.3f;
    public Color originalColor;
    public Color hurtColor;
    public float hiddenY = 100f;
    public float shownY = 0f;

    private bool healthShowing = true;

    FoxHealth foxHealth;
    FoxMovement foxMovement;
    PlayerInput input;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        foxHealth = player.GetComponent<FoxHealth>();
        foxMovement = player.GetComponent<FoxMovement>();
    }
    private void OnEnable()
    {
        foxHealth.playerHit.AddListener(LowerHealthUI);
    }
    void Start()
    {
        input = foxMovement.input;
        input.CharacterControls.ShowUI.performed += ToggleHealthUI;

        heartsGroupAlpha = heartsGroupPosition.GetComponent<CanvasGroup>();
    }

    private void OnDisable()
    {
        foxHealth.playerHit.RemoveListener(LowerHealthUI);

        input.CharacterControls.ShowUI.performed -= ToggleHealthUI;
    }


    void ToggleHealthUI(InputAction.CallbackContext context)
    {
        if (healthShowing)
            HideHealth();
        else
            ShowHealth();
    }
    public void ShowHealth()
    {
        if (!healthShowing)
        {
            healthShowing = true;
            //DOVirtual.Float(100, 0, 0.2f, MoveHearts);
            heartsGroupPosition.DOAnchorPosY(shownY, transitionTime).SetUpdate(true);
            heartsGroupAlpha.DOFade(1, transitionTime + 0.2f).SetUpdate(true).SetEase(Ease.InOutQuad);
        }
    }

    void HideHealth() 
    {
        heartsGroupPosition.DOAnchorPosY(hiddenY, transitionTime).SetUpdate(true);
        heartsGroupAlpha.DOFade(0, transitionTime - 0.1f).SetUpdate(true);
        healthShowing = false;
    }

    void LowerHealthUI(int health)
    {
        int newWidth = health/10 * 60 + 5;

        if (health > 0)
        {
            AnimateHeartsHurt(newWidth, false);
        }
        else
        {
            AnimateHeartsHurt(newWidth, true);
        }
    }

    void AnimateHeartsHurt(float newWidth, bool update)
    {
        Sequence movement = DOTween.Sequence();

        // Health mask width
        if (update)
            movement.Join(
              DOVirtual.Float(heartsMask.rect.width, newWidth, 0.2f, ChangeMaskWidth)
          ).OnComplete(ResetHealthBar);
        else
            movement.Join(
                DOVirtual.Float(heartsMask.rect.width, newWidth, 0.2f, ChangeMaskWidth) 
            );
        // Shake
        movement.Insert(0.1f,
                heartsGroupPosition.DOShakeAnchorPos(0.2f, 20f, 12)
        );

        // Heart color animation
        Sequence seq = DOTween.Sequence();
        seq.SetUpdate(update);
        foreach (Image image in heartsImages)
        {
            seq.Insert(0, image.DOColor(hurtColor, 0.2f).OnComplete(() => ResetHeartColor(image, update)));
        }

    }

    void ResetHeartColor(Image image, bool update)
    {
        image.DOColor(originalColor, 0.2f).SetUpdate(update);
    }

    void ChangeMaskWidth(float x)
    {
        heartsMask.sizeDelta = new Vector2(x, heartsMask.sizeDelta.y);
    }

    void ResetHealthBar()
    {
        Debug.Log("Reset health bar");
        heartsMask.sizeDelta = new Vector2(185, heartsMask.sizeDelta.y);
    }
}
