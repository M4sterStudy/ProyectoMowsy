using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using System; // Añadir esta línea para usar Exception

public class RoomCodeGenerator : MonoBehaviour
{
    DatabaseReference databaseReference;
    public TMPro.TextMeshProUGUI roomCodeText;
    public UnityEngine.UI.Button generateButton;

    void Start()
    {
        // Inicializar Firebase
        InitializeFirebase();

        // Configurar el botón para generar el código de sala
        if (generateButton != null)
        {
            generateButton.onClick.AddListener(GenerateAndStoreRoomCode);
        }
        else
        {
            Debug.LogError("El botón no está asignado en el Inspector.");
        }
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            if (task.Result == DependencyStatus.Available)
            {
                databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase inicializado correctamente.");
            }
            else
            {
                Debug.LogError("No se pudieron resolver todas las dependencias de Firebase: " + task.Result);
            }
        });
    }

    void GenerateAndStoreRoomCode()
    {
        try
        {
            // Generar un código aleatorio de 6 dígitos
            string roomCode = UnityEngine.Random.Range(100000, 999999).ToString();
            
            // Mostrar el código en pantalla usando TextMesh Pro
            if (roomCodeText != null)
            {
                roomCodeText.text = "Código de sala: " + roomCode;
            }
            else
            {
                Debug.LogError("El TextMeshProUGUI no está asignado en el Inspector.");
            }

            // Guardar el código en la base de datos temporalmente
            SaveRoomCodeToDatabase(roomCode);
        }
        catch (Exception ex) // Usar Exception desde el espacio de nombres System
        {
            Debug.LogError("Error al generar o almacenar el código de sala: " + ex.Message);
        }
    }

    void SaveRoomCodeToDatabase(string roomCode)
    {
        if (databaseReference != null)
        {
            string roomCodeId = "room_" + UnityEngine.Random.Range(1000, 9999).ToString();
            databaseReference.Child("roomCodes").Child(roomCodeId).SetValueAsync(roomCode).ContinueWithOnMainThread(task => {
                if (task.IsCompleted)
                {
                    Debug.Log("Código de sala guardado en la base de datos.");
                }
                else
                {
                    Debug.LogError("Error al guardar el código de sala en la base de datos: " + task.Exception);
                }
            });
        }
        else
        {
            Debug.LogError("Referencia a la base de datos no está inicializada.");
        }
    }
}
