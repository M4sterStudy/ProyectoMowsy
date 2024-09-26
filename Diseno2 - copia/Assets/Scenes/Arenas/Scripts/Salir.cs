using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Salir : MonoBehaviour
{
    // Este método puede ser llamado al hacer clic en el botón
    public void CerrarApp()
    {
        // Si estamos en el editor de Unity
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Detener el juego en el editor
        #else
        Application.Quit(); // Cerrar la aplicación en una build
        #endif
    }
}
