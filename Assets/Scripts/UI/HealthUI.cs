using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthUI : MonoBehaviour
{
    [Header("Assign in inspector")]
    public RectTransform heartsGroupPosition;
    public Image[] heartsImages;
    private CanvasGroup heartsGroupAlpha;
    public RectTransform heartsMask;

    [Space]
    [Header("Parameters")]
    public float timeToShow = 3f;
    private float timer = 0f;
    public float transitionTime = 0.3f;
    public Color originalColor;
    public Color hurtColor;

    private bool healthShowing = false;

    FoxHealth foxHealth;
    FoxMovement foxMovement;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        foxHealth = player.GetComponent<FoxHealth>();
        foxHealth.playerHit.AddListener(LowerHealthUI);
        foxHealth.playerDeath.AddListener(ResetHealthBar);

        foxMovement = player.GetComponent<FoxMovement>();

        heartsGroupAlpha = heartsGroupPosition.GetComponent<CanvasGroup>();

        // Init values
        heartsGroupAlpha.alpha = 0f;
        heartsGroupPosition.anchoredPosition = new Vector2(heartsGroupPosition.anchoredPosition.x, 100);
    }

    private void Update()
    {
        if(!foxMovement.isMoving && !healthShowing) 
        {
            timer += Time.deltaTime;

            if(timer > timeToShow)
            {
                timer = 0;
                healthShowing = true;
                ShowHealth(transitionTime);
            }
        }
        else if (foxMovement.isMoving && healthShowing)
        {
            timer = 0;
            HideHealth();
            healthShowing = false;
        }
        
    }

    void ShowHealth(float transitionTime)
    {
        //DOVirtual.Float(100, 0, 0.2f, MoveHearts);
        heartsGroupPosition.DOAnchorPosY(0, transitionTime).SetUpdate(true);
        heartsGroupAlpha.DOFade(1, transitionTime + 0.2f).SetUpdate(true);
    }

    void HideHealth() 
    {
        heartsGroupPosition.DOAnchorPosY(100, transitionTime).SetUpdate(true);
        heartsGroupAlpha.DOFade(0, transitionTime - 0.1f).SetUpdate(true);
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
        Invoke("HideHealth", 1.2f);
    }

    void AnimateHeartsHurt(float newWidth, bool update)
    {
        // Showing animation
        Sequence movement = DOTween.Sequence();
        movement.SetUpdate(update);
        movement.AppendCallback(() => ShowHealth(0.1f));

        // Health mask width
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
