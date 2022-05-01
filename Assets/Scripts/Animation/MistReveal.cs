using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMOD;

public class MistReveal : MonoBehaviour
{
    public GameObject[] mistObjects;
    public GameObject[] enemies;
    public GameObject colliders;
    public Collider trigger;

    public float startRevealValue = -1f;
    public float endRevealValue = 2f;
    public float transitionDuration = 5f;

    public FMODUnity.EventReference mistRevealEvent;
    private FMOD.Studio.EventInstance mistReveal;

    private List<Material> mistMaterials = new List<Material>();

    private int propertiesId;

    // Start is called before the first frame update
    void Start()
    {
        // Get reference of the readvalue property, get better performance calling the SetFloat of the material
        propertiesId = Shader.PropertyToID("_RevealValue");

        mistReveal = FMODUnity.RuntimeManager.CreateInstance(mistRevealEvent);
    }

    public void RevealMist() 
    {
        colliders.SetActive(true);
        trigger.enabled = false;

        for (int i = 0; i < mistObjects.Length; i++)
        {
            mistObjects[i].SetActive(true);
            mistMaterials.Add(mistObjects[i].GetComponent<MeshRenderer>().material);
        }

        for (int currentMaterial = 0; currentMaterial < mistObjects.Length; currentMaterial++)
        {
            int i = currentMaterial;
            DOVirtual.Float(startRevealValue, endRevealValue, transitionDuration, x=>ChangeMaterialRevealValue(x, i)).SetEase(Ease.Linear);
        }

        Invoke("SpawnEnemies", transitionDuration - 1f);

        mistReveal.start();
    }
    void SpawnEnemies() 
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].SetActive(true);
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            RevealMist();
        }
    }
}
