using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using FMOD;

public class MistReveal : MonoBehaviour
{
    [Header("Assign in Editor")]
    public GameObject[] mistObjects;
    public List<GameObject> enemies;
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

    private int remainingEnemies;

    // Start is called before the first frame update
    void Start()
    {
        // Get reference of the readvalue property, get better performance calling the SetFloat of the material
        propertiesId = Shader.PropertyToID("_RevealValue");

        player = GameObject.FindGameObjectWithTag("Player");
        foxHealth = player.GetComponent<FoxHealth>();
        //foxHealth.playerDeath.AddListener(HideMistSnap);
        foxHealth.playerRespawned.AddListener(HideMistSnap2);   // For when respawning from pause menu

        remainingEnemies = 0;

        foreach(GameObject enemy in enemies)
        {
            if (!enemy.GetComponent<EnemyBehaviour>().dead)
                remainingEnemies++;
        }
        
    }

    private void CheckRemainingEnemies() 
    {
        remainingEnemies = 0;
        foreach (GameObject enemy in enemies)
        {
            if (!enemy.GetComponent<EnemyBehaviour>().dead)
            {
                remainingEnemies++;
            }
        }
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
        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].SetActive(true);
        }
    }

    void DeSpawnEnemies()   // Resets or destroys enemies depending on their state
    {
        if (enemies.Count > 0)
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (enemies[i].GetComponent<EnemyBehaviour>().dead) // destroy and remove all dead enemies
                {
                    remainingEnemies--;
                    GameObject aux = enemies[i];
                    enemies.Remove(enemies[i]);
                    Destroy(aux);
                }
                else if (enemies[i].activeInHierarchy)  // Reset and hide all remaining enemies
                {
                    enemies[i].GetComponent<EnemyBehaviour>().Reset();
                    enemies[i].SetActive(false);
                }
            }
        }
    }

    public void HideMist() // Called when an enemy is dead
    {
        CheckRemainingEnemies();
        
        if (remainingEnemies == 0)
        {
            for (int currentMaterial = 0; currentMaterial < mistObjects.Length; currentMaterial++)
            {
                int i = currentMaterial;
                DOVirtual.Float(endRevealValue, startRevealValue, transitionDuration + 3f, x => ChangeMaterialRevealValue(x, i));
            }
            mistReveal.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            mistReveal.release();

            fog.Stop();

            Invoke("DisableMist", transitionDuration + 0.1f);
        }
    }

    public void HideMistSnap()  // Called when the player dies
    {
        Invoke("HideMistSnap2", 0.1f);
        /*if(fog.gameObject.activeInHierarchy)
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

        trigger.enabled = true;*/
    }

    private void HideMistSnap2()    // Called when the player dies
    {
        if (fog.gameObject.activeInHierarchy)
            for (int currentMaterial = 0; currentMaterial < mistObjects.Length; currentMaterial++)
            {
                int i = currentMaterial;
                DOVirtual.Float(endRevealValue, startRevealValue, 0.1f, x => ChangeMaterialRevealValue(x, i));
            }
        mistReveal.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        mistReveal.release();

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
        DeSpawnEnemies();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CheckRemainingEnemies();
            if(remainingEnemies > 0)
                RevealMist();
        }
    }
}
