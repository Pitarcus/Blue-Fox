using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    public Light directionalLight;
    public FoxHealth foxHealth;
    public LightingConditions preset;
    public float dayPeriod = 24;

    public bool rotateAllDirections = false;

    [SerializeField]private float timeOfDay;

    public Transform originalLightTransform;
    private Quaternion originalLightRotation;
    private Color originalFogColor;


    private void Start()
    {
        originalLightRotation = originalLightTransform.rotation;
        originalFogColor = RenderSettings.fogColor;
    }
    private void OnEnable()
    {
        if(foxHealth != null)
        foxHealth.playerRespawned.AddListener(Reset);
    }

    private void OnDisable()
    {
        if (foxHealth != null)
            foxHealth.playerRespawned.RemoveListener(Reset);
    }

    private void Update()
    {
        if (preset == null)
            return;
        if(Application.isPlaying)
        {
            timeOfDay += Time.deltaTime;
            timeOfDay %= dayPeriod;
            UpdateLighting(timeOfDay / dayPeriod);
        }
        else
        {
            UpdateLighting(timeOfDay / dayPeriod);
        }
    }
    private void UpdateLighting(float timePercent)
    {
        RenderSettings.ambientLight = preset.AmbientColor.Evaluate(timePercent);
        RenderSettings.fogColor = preset.FogColor.Evaluate(timePercent);

        if(directionalLight != null)
        {
            directionalLight.color = preset.DirectionalColor.Evaluate(timePercent);
            if(!rotateAllDirections)
                directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, -100f, 0));
            else
                directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) + 55f, (timePercent * 2 * 360f) - 45f, 0));
        }
    }

    private void OnValidate()
    {
        if(directionalLight != null)
        {
            return;
        }
        if(RenderSettings.sun != null)
        {
            directionalLight = RenderSettings.sun;
        }
        else
        {
            Light[] lights = FindObjectsOfType<Light>();
            foreach(Light light in lights)
            {
                if(light.type == LightType.Directional)
                {
                    directionalLight = light;
                    return;
                }
            }
        }
    }

    private void Reset()
    {
        //RenderSettings.ambientLight = preset.AmbientColor.Evaluate(0);
        //RenderSettings.fogColor = originalFogColor;

        //directionalLight.color = Color.white;
        //directionalLight.transform.localRotation = originalLightRotation;

        for(int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<EndSceneTrigger>().Reset();
        }

        //this.enabled = false;
    }
}
