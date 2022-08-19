using DG.Tweening;
using UnityEngine;

public class TutorialTriggerText : MonoBehaviour
{

    public CanvasGroup[] textCanvasGroup;

    private void OnTriggerEnter(Collider other)
    {
        ShowTextSequence();
    }
    public void ShowTextSequence()
    {
        Sequence sequence = DOTween.Sequence();

        foreach (CanvasGroup canvasGroup in textCanvasGroup)
        {
            sequence.Append(canvasGroup.DOFade(1, 2));
            sequence.AppendInterval(4f);
            sequence.Append(canvasGroup.DOFade(0, 2));
        }

        if(this.GetComponent<Collider>())
            this.GetComponent<Collider>().enabled = false;

        sequence.OnComplete(
            () => this.gameObject.SetActive(false));
        
    }
}
