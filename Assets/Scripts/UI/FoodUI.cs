using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class FoodUI : MonoBehaviour
{
    [Header("Assign in inspector")]
    public RectTransform foodGroupPosition;
    private CanvasGroup foodGroupAlpha;
    public TextMeshProUGUI foodAmountText;

    [Space]
    [Header("Parameters")]
    public float timeToShow = 3f;
    private float timer = 0f;
    public float transitionTime = 0.3f;
    public float hiddenY = -50f;
    public float shownY = 25f;

    private bool foodShowing = false;
    private bool updating = false;

    FoxFood foxFood;
    FoxMovement foxMovement;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        foxFood = player.GetComponent<FoxFood>();
        foxFood.foodChanged.AddListener(UpdateFoodUI);

        foxMovement = player.GetComponent<FoxMovement>();

        foodGroupAlpha = foodGroupPosition.GetComponent<CanvasGroup>();

        // Init values
        foodGroupAlpha.alpha = 0f;
        foodGroupPosition.anchoredPosition = new Vector2(foodGroupPosition.anchoredPosition.x, hiddenY);
    }
    private void OnDisable()
    {
        foxFood.foodChanged.RemoveListener(UpdateFoodUI);
    }

    private void Update()
    {
        if (!foxMovement.isMoving && !foodShowing)
        {
            timer += Time.deltaTime;

            if (timer > timeToShow)
            {
                timer = 0;
                ShowFood(transitionTime);
            }
        }
        else if (foxMovement.isMoving && foodShowing && !updating)
        {
            timer = 0;
            HideFood();
        }
    }

    void ShowFood(float transitionTime)
    {
        foodShowing = true;
        foodGroupPosition.DOAnchorPosY(shownY, transitionTime).SetUpdate(true);
        foodGroupAlpha.DOFade(1, transitionTime + 0.2f).SetUpdate(true).SetEase(Ease.InOutQuad);
    }

    void HideFood()
    {
        foodGroupPosition.DOAnchorPosY(hiddenY, transitionTime).SetUpdate(true);
        foodGroupAlpha.DOFade(0, transitionTime - 0.1f).SetUpdate(true);
        foodShowing = false;
    }

    void UpdateFoodUI(int foodAmount)
    {
        ShowFood(transitionTime);
        updating = true;
        // manage numbers particles and stuff
        foodAmountText.text = foodAmount.ToString();
        Invoke("StopUpdating", 0.5f);
    }

    void StopUpdating()
    {
        updating = false;
    }
}
