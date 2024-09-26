using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para gestionar las escenas

public class boton : MonoBehaviour
{
    // Método que será llamado al presionar el botón
    public void CambiarAEscena(string nombreEscena)
    {
        // Cargar la escena con el nombre especificado
        SceneManager.LoadScene(nombreEscena);
    }
}

