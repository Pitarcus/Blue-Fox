using UnityEngine.Events;
using UnityEngine;

public class GoldenApple : MonoBehaviour
{
    public MeshRenderer mr;
    public FMODUnity.EventReference appleIdleEvent;
    private FMOD.Studio.EventInstance appleIdleInstance;
    public ParticleSystem appleParticles;
    public ParticleSystem brokenParticles;
    public UnityEvent onAppleBroken;
    public EndScreenManager endScreen;

    public void PlayGoldenAppleLoop()
    {
        appleIdleInstance = FMODUnity.RuntimeManager.CreateInstance(appleIdleEvent);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(appleIdleInstance, transform);
        appleIdleInstance.start();
    }

    public void EndApple()
    {
        appleIdleInstance.setParameterByName("Apple_End", 1);
        appleIdleInstance.release();

        appleParticles.Stop();
        brokenParticles.Play();


        mr.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GetComponent<Collider>().enabled = false;

            endScreen.gameObject.SetActive(true);
            EndApple();

            InitialStrawberryManager.unlocked = new bool[10];

            onAppleBroken.Invoke();

            TimeManager.instance.SmoothStopTime(4f, false, 0);

            MusicManager.instance.StopEndSceneEvent();

            endScreen.WhiteScreen();
        }
    }
}
