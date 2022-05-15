using UnityEngine;
using UnityEngine.UI;

public class VCAController : MonoBehaviour
{
    private FMOD.Studio.VCA vca;

    public string vcaName;

    private Slider slider;

    void Start()
    {
        vca = FMODUnity.RuntimeManager.GetVCA("vca:/" + vcaName);
        slider = GetComponent<Slider>();
    }

    public void UpdateVCA(float volume)
    {
        vca.setVolume(volume);
    }
}
