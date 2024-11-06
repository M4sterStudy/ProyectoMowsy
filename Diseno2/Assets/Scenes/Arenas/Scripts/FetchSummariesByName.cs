using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Firestore;
using TMPro; // Para usar TextMeshPro
using UnityEngine.UI;

public class FetchSummariesByName : MonoBehaviour
{
    public TMP_InputField studentNameInputField; // Input donde el usuario ingresará el nombre del estudiante
    public Button fetchSummariesButton; // Botón para buscar los resúmenes
    public TMP_Text summariesDisplayText; // Texto donde se mostrarán los resúmenes obtenidos
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

        // Asegurarse de que el botón tenga un listener para buscar los resúmenes
        if (fetchSummariesButton != null)
        {
            fetchSummariesButton.onClick.AddListener(FetchSummariesFromDatabase);
        }
        else
        {
            Debug.LogError("El botón para buscar los resúmenes no está asignado.");
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

    // Método para buscar los resúmenes en Firestore
    public void FetchSummariesFromDatabase()
    {
        if (db == null)
        {
            Debug.LogError("Firestore no está inicializado.");
            return;
        }

        string studentName = studentNameInputField.text; // Obtener el nombre del estudiante ingresado

        if (string.IsNullOrEmpty(studentName))
        {
            Debug.LogError("Por favor, ingrese el nombre del estudiante.");
            return;
        }

        // Consultar Firestore para buscar los resúmenes asociados a ese nombre de estudiante
        Query query = db.Collection("summaries").WhereEqualTo("StudentName", studentName);
        query.GetSnapshotAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                QuerySnapshot snapshot = task.Result;

                if (snapshot.Count > 0)
                {
                    Debug.Log($"Se encontraron {snapshot.Count} resúmenes para el estudiante {studentName}.");

                    // Limpiar el texto de display antes de agregar los resúmenes
                    UnityMainThreadDispatcher.Instance().Enqueue(() => summariesDisplayText.text = "");

                    foreach (DocumentSnapshot document in snapshot.Documents)
                    {
                        Dictionary<string, object> summaryData = document.ToDictionary();

                        // Obtener los datos del resumen
                        string summaryId = summaryData["SummaryId"].ToString();
                        string content = summaryData["Content"].ToString();
                        string activityId = summaryData["ActivityId"].ToString();
                        Timestamp submissionDate = (Timestamp)summaryData["SubmissionDate"];

                        // Formatear el resumen para mostrarlo
                        string summaryText = $"Resumen ID: {summaryId}\n" +
                                             $"Contenido: {content}\n" +
                                             $"ID de Actividad: {activityId}\n" +
                                             $"Fecha de Envío: {submissionDate.ToDateTime()}\n\n";

                        // Mostrar el resumen en la UI (hilo principal)
                        UnityMainThreadDispatcher.Instance().Enqueue(() => summariesDisplayText.text += summaryText);
                    }
                }
                else
                {
                    Debug.LogError($"No se encontraron resúmenes para el estudiante {studentName}.");
                    UnityMainThreadDispatcher.Instance().Enqueue(() => summariesDisplayText.text = "No se encontraron resúmenes.");
                }
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error al consultar Firestore: " + task.Exception);
            }
        });
    }
}
