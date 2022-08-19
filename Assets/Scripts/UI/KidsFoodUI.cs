using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KidsFoodUI : MonoBehaviour
{

    public GameObject UICanvas;

    private void Start()
    {
        UICanvas.SetActive(false);
    }
    private void Update()
    {
        UICanvas.transform.LookAt(2 * transform.position - Camera.main.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("touching dad");
            TweenCanvasIn();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TweenCanvasOut();
        }
    }
    private void TweenCanvasIn()
    {
        UICanvas.SetActive(true);
        DOVirtual.Float(0f, 1f, 0.3f, SetCanvasScale).SetEase(Ease.InFlash).SetUpdate(true);
    }

    private void TweenCanvasOut()
    {
        DOVirtual.Float(1f, 0f, 0.3f, SetCanvasScale).SetEase(Ease.InFlash).OnComplete(() => UICanvas.SetActive(false));
    }

    private void SetCanvasScale(float x)
    {
        UICanvas.transform.localScale = new Vector3(x, x, x);
    }
}
