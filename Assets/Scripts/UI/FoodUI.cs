using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.InputSystem;

public class FoodUI : MonoBehaviour
{
    [Header("Assign in inspector")]
    public RectTransform foodGroupPosition;
    private CanvasGroup foodGroupAlpha;
    public TextMeshProUGUI foodAmountText;
    public HealthUI healthUI;

    [Space]
    [Header("Parameters")]
    public float transitionTime = 0.3f;
    public float hiddenY = -50f;
    public float shownY = 25f;

    private bool foodShowing = false;

    FoxFood foxFood;
    FoxMovement foxMovement;
    PlayerInput input;
    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        foxFood = player.GetComponent<FoxFood>();
        foxFood.foodChanged.AddListener(UpdateFoodUI);

        foxMovement = player.GetComponent<FoxMovement>();


        foodGroupAlpha = foodGroupPosition.GetComponent<CanvasGroup>();
    }
    void Start()
    {
        input = foxMovement.input;
        input.CharacterControls.ShowUI.performed += ToggleFoodUI;

        // Init values
        foodGroupAlpha.alpha = 0f;
        foodGroupPosition.anchoredPosition = new Vector2(foodGroupPosition.anchoredPosition.x, hiddenY);
    }
    private void OnDisable()
    {
        foxFood.foodChanged.RemoveListener(UpdateFoodUI);
        input.CharacterControls.ShowUI.performed -= ToggleFoodUI;
    }

    void ToggleFoodUI(InputAction.CallbackContext context) 
    {
        if (foodShowing)
            HideFood();
        else
            ShowFood();
    }

    public void ShowFood()
    {
        if (!foodShowing)
        {
            foodShowing = true;
            foodGroupPosition.DOAnchorPosY(shownY, transitionTime).SetUpdate(true);
            foodGroupAlpha.DOFade(1, transitionTime + 0.2f).SetUpdate(true).SetEase(Ease.InOutQuad);
        }
    }

    void HideFood()
    {
        foodGroupPosition.DOAnchorPosY(hiddenY, transitionTime).SetUpdate(true);
        foodGroupAlpha.DOFade(0, transitionTime - 0.1f).SetUpdate(true);
        foodShowing = false;
    }

    void UpdateFoodUI(int foodAmount)
    {
        ShowFood();
        healthUI.ShowHealth();
        // manage numbers particles and stuff
        foodAmountText.text = foodAmount.ToString();

        foodGroupPosition.DOShakePosition(0.2f).SetUpdate(true);
    }

}
