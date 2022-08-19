using UnityEngine;
using UnityEngine.Video;

public class VideoLogo : MonoBehaviour
{

    public VideoClip introClip;
    public VideoClip loopClip;
    public GameObject targetObject;

    public VideoPlayer videoPlayer;


    private void Start()
    {
        targetObject.SetActive(false);
        Invoke("StartLogoAnimation", 0.5f);
    }
    public void StartLogoAnimation()
    {
        targetObject.SetActive(true);
        videoPlayer.clip = introClip;
        videoPlayer.Play();
        videoPlayer.loopPointReached += StartLogoLoop;
        
    }

    void StartLogoLoop(VideoPlayer vp)
    {
        videoPlayer.clip = loopClip;
        videoPlayer.Play();
        videoPlayer.isLooping = true;
        videoPlayer.loopPointReached -= StartLogoLoop;
    }
}
