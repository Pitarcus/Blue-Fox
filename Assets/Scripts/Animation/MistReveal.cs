using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMOD;

public class MistReveal : MonoBehaviour
{
    [Header("Assign in Editor")]
    public GameObject[] mistObjects;
    public GameObject[] enemies;
    public GameObject colliders;
    public Collider trigger;
    public ParticleSystem fog;

    [Space]
    [Header("Magical Field & Enemies Spawn")]
    public float startRevealValue = -1f;
    public float endRevealValue = 2f;
    public float transitionDuration = 5f;

    // Sound Stuff
    public FMODUnity.EventReference mistRevealEvent;
    private FMOD.Studio.EventInstance mistReveal;

    private List<Material> mistMaterials = new List<Material>();

    private int propertiesId;

    private GameObject player;
    private FoxHealth foxHealth;

    // Start is called before the first frame update
    void Start()
    {
        // Get reference of the readvalue property, get better performance calling the SetFloat of the material
        propertiesId = Shader.PropertyToID("_RevealValue");

        player = GameObject.FindGameObjectWithTag("Player");
        foxHealth = player.GetComponent<FoxHealth>();
        foxHealth.playerDeath.AddListener(HideMistSnap);
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

        mistReveal = FMODUnity.RuntimeManager.CreateInstance(mistRevealEvent);

        mistReveal.start();
    }
    void SpawnEnemies() 
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].SetActive(true);
        }
    }

    void DeSpawnEnemies()
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].GetComponent<EnemyBehaviour>().Reset();
            enemies[i].SetActive(false);
        }
    }
    public void HideMist() 
    {
        for (int currentMaterial = 0; currentMaterial < mistObjects.Length; currentMaterial++)
        {
            int i = currentMaterial;
            DOVirtual.Float(endRevealValue, startRevealValue, transitionDuration + 3f, x => ChangeMaterialRevealValue(x, i));
        }
        mistReveal.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        mistReveal.release();

        fog.Stop();

        Invoke("DisableMist", transitionDuration + 3f);
    }

    public void HideMistSnap()
    {
        for (int currentMaterial = 0; currentMaterial < mistObjects.Length; currentMaterial++)
        {
            int i = currentMaterial;
            DOVirtual.Float(endRevealValue, startRevealValue, 0.1f, x => ChangeMaterialRevealValue(x, i));
        }
        mistReveal.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        mistReveal.release();

        DeSpawnEnemies();
        fog.Stop();

        DisableMist();

        trigger.enabled = true;
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
