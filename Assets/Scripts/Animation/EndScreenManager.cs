using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using UnityEngine.Rendering;

public class EndScreenManager : MonoBehaviour
{
    public FoxMovement foxMovement;
    public PlayerInputMixer playerInputMixer;
    public GameObject lastButton;
    public Image whiteTransition;
    public Sprite deadFoxSprite;
    public CanvasGroup foxHeads;
    public CanvasGroup finalTextGroup;
    public TextMeshProUGUI cubsText;
    public Image[] fillerImages;
    public Image[] faceImages;
    public ParticleSystem[] completeParticles;
    public ParticleSystem[] deadParticles;
    public FMODUnity.EventReference foxFacesFillingUpEvent;
    public FMODUnity.EventReference foxFaceComplete;
    public FMODUnity.EventReference foxFaceWrong;
    public FMODUnity.EventReference perfectEndEvent;
    public Volume hurtVolume;


    public int maxFoodAmount;

    public float whiteScreenTransitionTime = 4.5f;
    public float timeToShowHeads = 3f;
    public float showHeadsTime = 2f;
    public float timeToStartDecreasing = 2f;
    public float foodDecreaseInterval = 0.05f;
    public float timeToShowDead = 1f;
    public float timeToShowText = 2;

    public FoxFood foxFood;
    public Canvas HUD;
    public FoodUI foodUI;
    public HealthUI healthUI;

    private int finalFoodAmount;
    private int completeParticlesCounter = 0;
    private int savedCubs = 0;

    private void Start()
    {
        foreach (Image image in fillerImages)
        {
            image.fillAmount = 0;
        }
    }

    private void OnEnable()
    {
        foreach (Image image in fillerImages)
        {
            image.fillAmount = 0;
        }

        Color tempColor = whiteTransition.color;
        tempColor.a = 0f;
        whiteTransition.color = tempColor;

        foxHeads.alpha = 0f;
        finalTextGroup.alpha = 0f;
    }
    public void WhiteScreen()
    {
        foxMovement.input.UI.Enable();
        playerInputMixer.SwitchInputToUI();

        whiteTransition.DOFade(1, whiteScreenTransitionTime).OnComplete(ShowFoxHeads).SetUpdate(true);
    }

    private void ShowFoxHeads()
    {
        Sequence showFoxHeads = DOTween.Sequence().SetUpdate(true);
        showFoxHeads.AppendInterval(timeToShowHeads);
        showFoxHeads.Append(foxHeads.DOFade(1, showHeadsTime).OnComplete(ShowGatheredFood));
    }

    public void ShowGatheredFood()
    {
        CanvasGroup HUDCanvas = HUD.GetComponent<CanvasGroup>();
        HUDCanvas.alpha = 0f;
        HUDCanvas.DOFade(1f, 1f).SetUpdate(true);

        HUD.sortingOrder = 4;

        foodUI.ShowFood();
        healthUI.heartsGroupAlpha.alpha = 0;

        finalFoodAmount = foxFood.GetFoodAmount();

        CalculateFilling();

        StartCoroutine("CountFood");


    }

    void CalculateFilling()
    {
        float fillAmount1 = 0f;
        float fillAmount2 = 0f;
        float fillAmount3 = 0f;

        if (finalFoodAmount >= 40)
        {
            fillAmount1 = 1f;
            fillAmount2 = 1f;
            fillAmount3 = (finalFoodAmount - 40f) / 20f;
            savedCubs = 3;
        }
        else if (finalFoodAmount >= 20)
        {
            fillAmount1 = 1f;
            fillAmount2 = (finalFoodAmount - 20f) / 20f;
            fillAmount3 = 0f;

            savedCubs = 2;
        }
        else if(finalFoodAmount >= 1)
        {
            fillAmount1 = finalFoodAmount / 20f;
            fillAmount2 = 0;
            fillAmount3 = 0f;

            savedCubs = 1;
        }
        else
        {
            savedCubs = 0;
        }

        // Animate faces filling
        Sequence facesFilling = DOTween.Sequence();
        facesFilling.SetUpdate(true);

        facesFilling.AppendInterval(timeToStartDecreasing);

        if(fillAmount1 > 0)
        {
            facesFilling.AppendCallback(PlayFillingSound);
        }

        facesFilling.Append(DOVirtual.Float(0, fillAmount1, fillAmount1 * 20 * foodDecreaseInterval, ChangeFillAmount1).SetEase(Ease.Linear));
        facesFilling.AppendCallback(PlayNextFaceParticles);
        facesFilling.AppendInterval(0f);

        if (fillAmount2 > 0)
        {
            facesFilling.AppendCallback(PlayFillingSound);
        }

        facesFilling.Append(DOVirtual.Float(0, fillAmount2, fillAmount2 * 20 * foodDecreaseInterval, ChangeFillAmount2).SetEase(Ease.Linear));
        facesFilling.AppendCallback(PlayNextFaceParticles);
        facesFilling.AppendInterval(0f);

        if (fillAmount3 > 0)
        {
            facesFilling.AppendCallback(PlayFillingSound);
        }

        facesFilling.Append(DOVirtual.Float(0, fillAmount3, fillAmount3 * 20 * foodDecreaseInterval, ChangeFillAmount3).SetEase(Ease.Linear));
        facesFilling.AppendCallback(PlayNextFaceParticles);
    }

    void PlayFillingSound()
    {
        FMODUnity.RuntimeManager.PlayOneShot(foxFacesFillingUpEvent);
    }

    void PlayNextFaceParticles()
    {
        if(completeParticlesCounter == 0 && savedCubs > 0)
        {
            completeParticles[0].Stop();
            completeParticles[0].Play();
            FMODUnity.RuntimeManager.PlayOneShot(foxFaceComplete);
        }
        else if(completeParticlesCounter == 1 && savedCubs > 1)
        {
            completeParticles[1].Stop();
            completeParticles[1].Play();
            FMODUnity.RuntimeManager.PlayOneShot(foxFaceComplete);
        }
        else if (completeParticlesCounter == 2 && savedCubs > 2)
        {
            completeParticles[2].Stop();
            completeParticles[2].Play();
            FMODUnity.RuntimeManager.PlayOneShot(foxFaceComplete);
        }
        completeParticlesCounter++;
    }

    IEnumerator CountFood()
    {
        yield return new WaitForSecondsRealtime(timeToStartDecreasing);

        // Do the animation of lowering the food number and filling up the foxes

        for (int i = 0; i < finalFoodAmount; i++)
        {
            foxFood.DecreaseFoodAmount();
            yield return new WaitForSecondsRealtime(foodDecreaseInterval);
        }

        yield return new WaitForSecondsRealtime(timeToShowDead);

        CalculateTextAndAnimateDead();

        if (savedCubs == 3)
            FMODUnity.RuntimeManager.PlayOneShot(perfectEndEvent);

        yield return new WaitForSecondsRealtime(timeToShowText);

        ShowEndText();
    }

    private void CalculateTextAndAnimateDead()
    {
        // Text
        cubsText.text = "You saved ";

        if (finalFoodAmount > 40)
        {
            cubsText.text += "3";
        }
        else if (finalFoodAmount > 20)
        {
            cubsText.text += "2";

            faceImages[2].rectTransform.DOShakePosition(0.5f, 3).SetUpdate(true);

            faceImages[2].sprite = deadFoxSprite;
            faceImages[2].color = Color.red;

            deadParticles[2].Stop();
            deadParticles[2].Play();

            FMODUnity.RuntimeManager.PlayOneShot(foxFaceWrong);

            Sequence volume = DOTween.Sequence().SetUpdate(true);
            volume.Append(DOVirtual.Float(0, 1, 0.2f, HurtVolume));
            volume.Append(DOVirtual.Float(1, 0, 0.2f, HurtVolume));
        }
        else if (finalFoodAmount >= 1)
        {
            cubsText.text += "1";

            faceImages[1].rectTransform.DOShakePosition(0.5f, 3).SetUpdate(true);
            faceImages[2].rectTransform.DOShakePosition(0.5f, 3).SetUpdate(true);

            faceImages[1].sprite = deadFoxSprite;
            faceImages[2].sprite = deadFoxSprite;

            faceImages[1].color = Color.red;
            faceImages[2].color = Color.red;

            deadParticles[1].Stop();
            deadParticles[1].Play();
            deadParticles[2].Stop();
            deadParticles[2].Play();

            FMODUnity.RuntimeManager.PlayOneShot(foxFaceWrong);
        }
        else
        {
            cubsText.text += "0";

            faceImages[0].rectTransform.DOShakePosition(0.5f, 3).SetUpdate(true);
            faceImages[1].rectTransform.DOShakePosition(0.5f, 3).SetUpdate(true);
            faceImages[2].rectTransform.DOShakePosition(0.5f, 3).SetUpdate(true);

            faceImages[0].sprite = deadFoxSprite;
            faceImages[1].sprite = deadFoxSprite;
            faceImages[2].sprite = deadFoxSprite;

            faceImages[0].color = Color.red;
            faceImages[1].color = Color.red;
            faceImages[2].color = Color.red;

            deadParticles[0].Stop();
            deadParticles[0].Play();
            deadParticles[1].Stop();
            deadParticles[1].Play();
            deadParticles[2].Stop();
            deadParticles[2].Play();

            FMODUnity.RuntimeManager.PlayOneShot(foxFaceWrong);
        }

        cubsText.text += " of your cubs.";
    }

    void HurtVolume(float x)
    {
        hurtVolume.weight = x;
    }
    private void ShowEndText()
    {
        // Particle Animation for dead

        
        finalTextGroup.DOFade(1, 0.8f).SetUpdate(true);

        EventSystem.current.SetSelectedGameObject(lastButton);
    }

    private void ChangeFillAmount1(float x)
    {
        fillerImages[0].fillAmount = x;
    }
    private void ChangeFillAmount2(float x)
    {
        fillerImages[1].fillAmount = x;
    }
    private void ChangeFillAmount3(float x)
    {
        fillerImages[2].fillAmount = x;
    }
}
