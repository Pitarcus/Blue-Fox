using DG.Tweening;
using UnityEngine;

public class EndSceneTrigger : MonoBehaviour
{

    [SerializeField]
    private Collider trigger;
    [SerializeField]
    private LightingManager lightingManager;
    [SerializeField]
    private Material newSkyboxMaterial;
    [SerializeField]
    private GameObject clouds;

    private static bool entered = false;
    private static float currentLightSpeed = 25f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!entered)
            {
                trigger.enabled = false;

                // Show last Clouds and change skybox material
                clouds.SetActive(true);
                RenderSettings.skybox = newSkyboxMaterial;

                // Control light rotation speed and change of color through lighting manager
                lightingManager.enabled = true;
                SetLightSpeed(currentLightSpeed);

                entered = true;
            }
            else
            {
                trigger.enabled = false;

                SetLightSpeed(currentLightSpeed - 5);
                currentLightSpeed = currentLightSpeed - 5;
            }
        }

    }
    private void SetLightSpeed(float x )
    {
        lightingManager.dayPeriod = x;
    }

}
