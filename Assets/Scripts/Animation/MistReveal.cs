using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MistReveal : MonoBehaviour
{

    public float startRevealValue = -1f;
    public float endRevealValue = 2f;
    public float transitionDuration = 5f;

    public GameObject[] mistObjects;

    public GameObject colliders;
    private List<Material> mistMaterials = new List<Material>();

    private int propertiesId;

    // Start is called before the first frame update
    void Start()
    {
        // Get reference of the readvalue property, get better performance calling the SetFloat of the material
        propertiesId = Shader.PropertyToID("_RevealValue");
    }

    public void RevealMist() 
    {
        colliders.SetActive(true);

        for (int i = 0; i < mistObjects.Length; i++)
        {
            mistObjects[i].SetActive(true);
            mistMaterials.Add(mistObjects[i].GetComponent<MeshRenderer>().material);
        }

        for (int currentMaterial = 0; currentMaterial < mistObjects.Length; currentMaterial++)
        {
            int i = currentMaterial;
            DOVirtual.Float(startRevealValue, endRevealValue, transitionDuration, x=>ChangeMaterialRevealValue(x, i));
        }
    }

    public void HideMist() 
    {
        for (int currentMaterial = 0; currentMaterial < mistObjects.Length; currentMaterial++)
        {
            int i = currentMaterial;
            DOVirtual.Float(endRevealValue, startRevealValue, transitionDuration + 3f, x => ChangeMaterialRevealValue(x, i));
        }
        Invoke("DisableMist", transitionDuration + 3f);
    }

    private void ChangeMaterialRevealValue(float x, int material) 
    {
        mistMaterials[material].SetFloat(propertiesId, x);
    }

    private void DisableMist()
    {
        for (int i = 0; i < mistObjects.Length; i++)
        {
            mistObjects[i].SetActive(false);
        }
        colliders.SetActive(false);
    }
}
