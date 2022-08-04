using UnityEngine;
using UnityEngine.UI;

public class VCAController : MonoBehaviour
{
    private FMOD.Studio.VCA vca;

    public string vcaName;

    private Slider slider;

    void Awake()
    {
        vca = FMODUnity.RuntimeManager.GetVCA("vca:/" + vcaName);
        slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        vca.getVolume(out float volume);
        slider.value = volume;
    }

    public void UpdateVCA(float volume)
    {
        vca.setVolume(volume);
    }
}
