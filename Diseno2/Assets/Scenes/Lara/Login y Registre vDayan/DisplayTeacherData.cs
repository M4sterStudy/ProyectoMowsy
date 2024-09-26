using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayTeacherData : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText; // Campo TMP_Text donde se mostrar�n todos los datos

    void Start()
    {
        if (UserManager.Instance != null)
        {
            UserManager.Instance.OnTeacherDataLoaded += MostrarDatosProfesor;
            if (UserManager.Instance.TeacherData != null)
            {
                MostrarDatosProfesor();
            }
        }
        else
        {
            Debug.LogWarning("UserManager instance not found.");
        }
    }

    private void MostrarDatosProfesor()
    {
        if (UserManager.Instance != null && UserManager.Instance.TeacherData != null)
        {
            Dictionary<string, object> teacherData = UserManager.Instance.TeacherData;

            // Obtener los datos y verificar si existen
            string nombre = teacherData.ContainsKey("Nombre") ? teacherData["Nombre"].ToString() : "Desconocido";
            string apellido = teacherData.ContainsKey("Apellido") ? teacherData["Apellido"].ToString() : "Desconocido";

            // Concatenar todos los datos en un solo string
            string datosCompletos = $"Bienvenido: {nombre}" +
                                    $" {apellido}" ;

            // Asignar el texto concatenado al TMP_Text
            infoText.text = datosCompletos;
            Debug.Log("Datos del profesor mostrados en un solo campo de texto.");
        }
        else
        {
            Debug.LogWarning("No se encontr� la instancia de UserManager o los datos del profesor no est�n disponibles.");
            infoText.text = "No se encontraron datos del profesor.";
        }
    }
}