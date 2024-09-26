using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoControl : MonoBehaviour
{
    public Button forwardButton;
    public Button rewindButton;
    public double skipTime = 10.0;

    void Start()
    {

        forwardButton.onClick.AddListener(Forward10Seconds);
        rewindButton.onClick.AddListener(Rewind10Seconds);
    }

    public void Forward10Seconds()
    {
        VideoPlayer videoPlayer = FindObjectOfType<VideoPlayer>();
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.time = Mathf.Min((float)(videoPlayer.time + skipTime), (float)videoPlayer.length);
        }
    }


    public void Rewind10Seconds()
    {
        VideoPlayer videoPlayer = FindObjectOfType<VideoPlayer>();
        if (videoPlayer != null && videoPlayer.isPlaying)
        {
            videoPlayer.time = Mathf.Max((float)(videoPlayer.time - skipTime), 0);
        }
    }
}



