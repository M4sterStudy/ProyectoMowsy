using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayStudentData : MonoBehaviour
{
    [SerializeField] private TMP_Text infoText; // Campo TMP_Text donde se mostrar�n todos los datos del estudiante

    void Start()
    {
        if (UserManager.Instance != null)
        {
            // Suscribirse al evento correcto para los estudiantes
            UserManager.Instance.OnStudentDataLoaded += MostrarDatosEstudiante;

            // Verificar si los datos del estudiante ya est�n cargados y disponibles
            if (UserManager.Instance.StudentData != null)
            {
                MostrarDatosEstudiante();
            }
        }
        else
        {
            Debug.LogWarning("UserManager instance not found.");
        }
    }

    private void MostrarDatosEstudiante()
    {
        if (UserManager.Instance != null && UserManager.Instance.StudentData != null)
        {
            Dictionary<string, object> studentData = UserManager.Instance.StudentData;

            // Obtener los datos y verificar si existen
            string nombre = studentData.ContainsKey("Nombre") ? studentData["Nombre"].ToString() : "Desconocido";
            string apellido = studentData.ContainsKey("Apellido") ? studentData["Apellido"].ToString() : "Desconocido";
        
            // Concatenar todos los datos en un solo string
            string datosCompletos = $"Bienvenido: {nombre}" +
                                    $" {apellido}";

            // Asignar el texto concatenado al TMP_Text
            infoText.text = datosCompletos;
            Debug.Log("Datos del estudiante mostrados correctamente.");
        }
        else
        {
            Debug.LogWarning("No se encontr� la instancia de UserManager o los datos del estudiante no est�n disponibles.");
            infoText.text = "No se encontraron datos del estudiante.";
        }
    }
}