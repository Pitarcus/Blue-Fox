using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private GameObject player;
    private FoxMovement foxMovement;
    private FoxHealth foxHealth;
    public Animator sceneUIAnimation;
    public PauseMenuAnimation pauseScript;

    private bool respawning;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        foxMovement = player.GetComponent<FoxMovement>();
        foxHealth = player.GetComponent<FoxHealth>();
    }

    private void Start()
    {
        respawning = false;
    }

    public void RespawnPlayerFromPause()
    {
        if (!respawning)
        {
            respawning = true;

            pauseScript.DisableSelectedButtons();

            sceneUIAnimation.SetTrigger("Start");

            StartCoroutine(foxHealth.ReSpawnPlayer(false, 1.5f));

            StartCoroutine("WaitSpawn");
        }
    }

    IEnumerator WaitSpawn()
    {
        yield return new WaitForSecondsRealtime(2f);

        sceneUIAnimation.SetTrigger("End");

        pauseScript.HidePauseMenu();

        respawning = false;
    }
}
