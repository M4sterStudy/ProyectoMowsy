using UnityEngine;
using UnityEngine.Video;
using TMPro; 
using System.Collections.Generic;

public class ChangeVideoSpeed : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public TMP_Dropdown speedDropdown;

    void Start()
    {

        videoPlayer = GetComponent<VideoPlayer>();


        speedDropdown.ClearOptions();
        speedDropdown.AddOptions(new List<string> { "0.5x","1x", "1.25x", "1.5x", "2x" });


        speedDropdown.onValueChanged.AddListener(ChangePlaybackSpeed);
    }


    public void ChangePlaybackSpeed(int index)
    {
        switch (index)
        {
            case 0:
                videoPlayer.playbackSpeed = 0.5f;
                break;

            case 1:
                videoPlayer.playbackSpeed = 1.0f;
                break;
            case 2:
                videoPlayer.playbackSpeed = 1.25f;
                break;
            case 3:
                videoPlayer.playbackSpeed = 1.5f;
                break;
            case 4:
                videoPlayer.playbackSpeed = 2.0f;
                break;
            default:
                videoPlayer.playbackSpeed = 1.0f; 
                break;
        }
    }
}




