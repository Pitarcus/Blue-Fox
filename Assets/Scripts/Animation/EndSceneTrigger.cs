using DG.Tweening;
using UnityEngine;

public class EndSceneTrigger : MonoBehaviour
{
    [SerializeField]
    private LightingManager lightingManager;
    [SerializeField]
    private Material newSkyboxMaterial;
    [SerializeField]
    private GameObject clouds;
    [SerializeField]
    private GameObject starParticles;
    [SerializeField]
    private ParticleSystem appleParticles;
    [SerializeField]
    private GoldenApple goldenAppleScript;

    private bool entered = false;

    private Material originalSkybox;

    private void Awake()
    {
        originalSkybox = RenderSettings.skybox;
    }

    private void Update()
    {
        if(entered)
        {
            MusicManager.instance.UpdateEndDistance();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!entered)
            {
                // Show last Clouds and change skybox material
                clouds.SetActive(true);
                RenderSettings.skybox = newSkyboxMaterial;

                appleParticles.Play();
                goldenAppleScript.PlayGoldenAppleLoop();

                MusicManager.instance.PauseOutOfTheCave();
                MusicManager.instance.PlayEndSceneEvent();

                // Control light rotation speed and change of color through lighting manager
                lightingManager.enabled = true;

                entered = true;
            }
        }
    }

    public void Reset()
    {
        //entered = false;
        //clouds.SetActive(false);
        //appleParticles.Stop();

        MusicManager.instance.ResetEndDistance();

        //RenderSettings.skybox = originalSkybox;
    }
}
