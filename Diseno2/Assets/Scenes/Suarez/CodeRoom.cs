using System;
using UnityEngine;
using Firebase;
using Firebase.Firestore; // Usar Firebase Firestore para almacenar datos
using TMPro; // Para usar TextMeshPro
using UnityEngine.UI; // Para utilizar UI de botones
using System.Collections;
using System.Collections.Generic;

public class CodeRoom : MonoBehaviour
{
    public TMP_Text roomCodeText; // Texto donde se mostrará el código de la sala
    public Button generateCodeButton; // Referencia al botón que genera el código de la sala
    private FirebaseFirestore db; // Referencia a Firestore
    private string roomCode; // Almacena el código de sala generado
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

        // Asegurarse de que el botón tenga un listener para generar el código al hacer clic
        if (generateCodeButton != null)
        {
            generateCodeButton.onClick.AddListener(GenerateAndStoreRoomCode);
        }
        else
        {
            Debug.LogError("El botón para generar el código de sala no está asignado.");
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

    // Método que se ejecuta cuando el usuario presiona el botón para generar el código de sala
    public void GenerateAndStoreRoomCode()
    {
        if (db == null)
        {
            Debug.LogError("Firestore no está inicializado.");
            return;
        }

        // Generar un código aleatorio de 4 dígitos
        roomCode = UnityEngine.Random.Range(1000, 9999).ToString();
        Debug.Log($"Código de sala generado: {roomCode}");

        // Guardar el código en Firestore
        CreateRoomDocument(roomCode);
        // Mostrar el código en la UI
        DisplayRoomCode();
    }

    // Método para guardar el código de sala en Firestore
    private void CreateRoomDocument(string code)
    {
        Debug.Log("Guardando código de sala en Firestore...");

        // Crear un ID único para la sala
        string roomId = db.Collection("rooms").Document().Id;

        // Obtener el ID del profesor desde UserManager
        string professorId = UserManager.Instance != null ? UserManager.Instance.TeacherDataID : "Desconocido";
        Debug.Log($"ID_Profesor obtenido para guardar en la sala: {professorId}"); // Verifica que tenga el valor correcto

        // Crear un diccionario para los datos de la sala
        Dictionary<string, object> roomData = new Dictionary<string, object>
    {
        { "RoomCode", code },
        { "Fechacreacion", DateTime.Now.ToString() },
        { "Estado", "Activa" },
        { "ID_Profesor", professorId }, // Almacena el ID del profesor
        { "Estudiantes", new List<Dictionary<string, string>>() }, // Lista vacía para los estudiantes
        { "ID_Tema_Seleccionado", null }, // Inicialmente vacío
        { "ID_Texto_Seleccionado", null } // Inicialmente vacío
    };

        // Guardar los datos en Firestore
        db.Collection("rooms").Document(roomId).SetAsync(roomData).ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log($"Código de sala guardado exitosamente en Firestore con ID_Profesor: {professorId}");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error al guardar el código de sala en Firestore.");
                foreach (var e in task.Exception.Flatten().InnerExceptions)
                {
                    Debug.LogError("Error detallado: " + e.Message);
                }
            }
        });
    }



    // Método para mostrar el código de sala en la UI
    public void DisplayRoomCode()
    {
        if (!string.IsNullOrEmpty(roomCode))
        {
            // Aquí aseguramos que el código se muestre en el hilo principal
            UnityMainThreadDispatcher.Instance().Enqueue(() => {
                roomCodeText.text = roomCode;
                Debug.Log("Código de sala mostrado en la UI: " + roomCode);
            });
        }
        else
        {
            Debug.LogError("No hay código de sala generado para mostrar.");
        }
    }
}
