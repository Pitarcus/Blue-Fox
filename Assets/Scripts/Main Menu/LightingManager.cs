using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class LightingManager : MonoBehaviour
{
    public Light directionalLight;
    public LightingConditions preset;
    public float dayPeriod = 24;

    [SerializeField]private float timeOfDay;


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
            directionalLight.transform.localRotation = Quaternion.Euler(new Vector3((timePercent * 360f) - 90f, -100f, 0));
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
}
