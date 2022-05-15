using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelGraphics : MonoBehaviour
{
    public GameObject pixelSetUp;
    public Camera textureCamera;

    public RenderTexture renderTexture;

    public void ChoosePixel(int option)
    {
        Debug.Log(option);
        if (option == 0)
            SetPixelCamera();
        else
            DisablePixelCamera();
    }
    private void SetPixelCamera()
    {
        pixelSetUp.SetActive(true);
        textureCamera.targetTexture = renderTexture;
    }

    private void DisablePixelCamera()
    {
        pixelSetUp.SetActive(false);
        textureCamera.targetTexture = null;
    }
}
