using System;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SaveSummary : MonoBehaviour
{
    public TMP_InputField studentIdInputField; // Input donde el usuario ingresará el ID del estudiante
    public TMP_InputField activityIdInputField; // Input donde el usuario ingresará el ID de la actividad
    public TMP_InputField contentInputField; // Input donde el usuario ingresará el contenido del resumen
    public Button saveSummaryButton; // Botón para guardar el resumen
    private FirebaseFirestore db; // Referencia a Firestore
    private FirebaseInitializer firebaseInitializer; // Referencia al inicializador de Firebase

    void Start()
    {
        // Buscar el FirebaseInitializer en la escena
        firebaseInitializer = FindObjectOfType<FirebaseInitializer>();

        if (firebaseInitializer == null)
        {
            Debug.LogError("FirebaseInitializer no encontrado en la escena.");
            return;
        }

        // Esperar hasta que Firebase esté inicializado antes de proceder
        StartCoroutine(WaitForFirebaseInitialization());

        // Asegurarse de que el botón tenga un listener para guardar el resumen
        if (saveSummaryButton != null)
        {
            saveSummaryButton.onClick.AddListener(SaveSummaryToDatabase);
        }
        else
        {
            Debug.LogError("El botón para guardar el resumen no está asignado.");
        }
    }

    private IEnumerator WaitForFirebaseInitialization()
    {
        while (!firebaseInitializer.IsInitialized)
        {
            Debug.Log("Esperando la inicialización de Firebase...");
            yield return new WaitForSeconds(0.5f);
        }

        db = firebaseInitializer.Db; // Obtener la referencia de Firestore de FirebaseInitializer
        Debug.Log("Firestore inicializado correctamente");
    }

    // Método para guardar el resumen en Firestore
    public void SaveSummaryToDatabase()
    {
        if (db == null)
        {
            Debug.LogError("Firestore no está inicializado.");
            return;
        }

        // Generar un ID aleatorio único para el resumen
        string summaryId = UnityEngine.Random.Range(100000, 999999).ToString();

        // Obtener los datos ingresados por el usuario
        string studentId = studentIdInputField.text;
        string activityId = activityIdInputField.text;
        string content = contentInputField.text;
        Timestamp timestamp = Timestamp.FromDateTime(DateTime.UtcNow); // Obtener la fecha y hora actual como timestamp

        // Verificar que los campos no estén vacíos
        if (string.IsNullOrEmpty(studentId) || string.IsNullOrEmpty(activityId) || string.IsNullOrEmpty(content))
        {
            Debug.LogError("Todos los campos son obligatorios.");
            return;
        }

        // Crear un diccionario con los datos del resumen
        Dictionary<string, object> summaryData = new Dictionary<string, object>
        {
            { "SummaryId", summaryId },
            { "StudentId", studentId },
            { "ActivityId", activityId },
            { "Content", content },
            { "SubmissionDate", timestamp }
        };

        // Guardar los datos en Firestore
        db.Collection("summaries").Document(summaryId).SetAsync(summaryData).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("Resumen guardado exitosamente en Firestore.");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error al guardar el resumen en Firestore: " + task.Exception);
            }
        });
    }
}
