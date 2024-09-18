using UnityEngine;
using UnityEngine.Video;
using TMPro; // Importar el espacio de nombres de TextMesh Pro
using System.Collections.Generic;

public class ChangeVideoSpeed : MonoBehaviour
{
    private VideoPlayer videoPlayer;  // Variable para almacenar la referencia al VideoPlayer
    public TMP_Dropdown speedDropdown;  // Referencia al TMP_Dropdown que controla la velocidad

    void Start()
    {
        // Obtener automáticamente el VideoPlayer en el mismo GameObject
        videoPlayer = GetComponent<VideoPlayer>();

        // Inicializar las opciones en el TMP_Dropdown
        speedDropdown.ClearOptions();
        speedDropdown.AddOptions(new List<string> { "0.5x", "1.25x", "1.5x", "2x" });

        // Suscribir el evento de cambio de valor del TMP_Dropdown
        speedDropdown.onValueChanged.AddListener(ChangePlaybackSpeed);
    }

    // Función para cambiar la velocidad de reproducción
    public void ChangePlaybackSpeed(int index)
    {
        switch (index)
        {
            case 0:
                videoPlayer.playbackSpeed = 0.5f;
                break;
            case 1:
                videoPlayer.playbackSpeed = 1.25f;
                break;
            case 2:
                videoPlayer.playbackSpeed = 1.5f;
                break;
            case 3:
                videoPlayer.playbackSpeed = 2.0f;
                break;
            default:
                videoPlayer.playbackSpeed = 1.0f; // Velocidad normal por defecto
                break;
        }
    }
}




