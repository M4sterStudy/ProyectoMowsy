using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject objetoConVideo;  // El objeto que contiene el Video Player
    private VideoPlayer videoPlayer;

    void Start()
    {
        // Obtén el componente Video Player del objeto
        videoPlayer = objetoConVideo.GetComponent<VideoPlayer>();

        // Asegúrate de que el objeto esté desactivado al inicio
        objetoConVideo.SetActive(false);
    }

    public void ActivarYReproducirVideo()
    {
        // Activa el objeto
        objetoConVideo.SetActive(true);

        // Reproduce el video cuando el objeto se activa
        if (videoPlayer != null)
        {
            videoPlayer.Play();
        }
    }
}
