using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelGraphics : MonoBehaviour
{
    public GameObject pixelSetUp;
    public Camera textureCamera;

    public RenderTexture renderTexture;

    public static bool pixel = true;

    private int originakMask;

    private void Awake()
    {
        originakMask = textureCamera.cullingMask;

        if(pixel)
        {
            SetPixelCamera();
        }
        else
        {
            DisablePixelCamera();
        }
    }

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
        pixel = true;
        pixelSetUp.SetActive(true);
        textureCamera.targetTexture = renderTexture;

        textureCamera.cullingMask |= (0 << LayerMask.NameToLayer("UI"));
    }

    private void DisablePixelCamera()
    {
        pixel = false;
        pixelSetUp.SetActive(false);
        textureCamera.targetTexture = null;

        textureCamera.cullingMask = -1;
    }
}
