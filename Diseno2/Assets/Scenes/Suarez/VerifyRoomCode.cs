using System;
using UnityEngine;
using Firebase.Firestore; // Usar Firebase Firestore
using TMPro; // Para usar TextMeshPro
using UnityEngine.SceneManagement; // Para cambiar escenas
using UnityEngine.UI; // Para usar UI de botones
using System.Collections;
using System.Collections.Generic;
public class VerifyRoomCode : MonoBehaviour
{
    public TMP_InputField codeInputField; // Input donde el usuario ingresará el código
    public Button verifyCodeButton; // Botón para verificar el código
    private FirebaseFirestore db; // Referencia a Firestore
    private FirebaseInitializer firebaseInitializer; // Referencia al inicializador de Firebase
    public string targetSceneName; // Nombre de la escena a la que se cambiará si el código es correcto
    private bool prueba = false;

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

        // Asegurarse de que el botón tenga un listener para verificar el código al hacer clic
        if (verifyCodeButton != null)
        {
            verifyCodeButton.onClick.AddListener(() => StartCoroutine(VerifyRoomCodeAndChangeScene()));
        }
        else
        {
            Debug.LogError("El botón para verificar el código no está asignado.");
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

    // Método que verifica el código y espera a que se complete
    private IEnumerator VerifyRoomCodeAndChangeScene()
    {
        yield return StartCoroutine(VerifyRoomCodeInDatabase());

        if (prueba)
        {
            ChangeScene();
        }
    }

    // Método para verificar el código ingresado
    private IEnumerator VerifyRoomCodeInDatabase()
    {
        if (db == null)
        {
            Debug.LogError("Firestore no está inicializado.");
            yield break;
        }

        string enteredCode = codeInputField.text; // Obtener el código ingresado por el usuario

        if (string.IsNullOrEmpty(enteredCode))
        {
            Debug.LogError("Por favor, ingrese un código.");
            yield break;
        }

        // Consultar Firestore para buscar si el código existe
        Query query = db.Collection("rooms").WhereEqualTo("RoomCode", enteredCode);
        var task = query.GetSnapshotAsync();

        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsFaulted)
        {
            Debug.LogError("Error al consultar Firestore: " + task.Exception);
            yield break;
        }

        if (task.Result.Count > 0)
        {
            Debug.Log("Código encontrado en la base de datos.");
            prueba = true; // Establecer prueba en true

            // Obtener el ID y el nombre del estudiante desde UserManager
            string studentId = UserManager.Instance?.StudentDataID;
            string studentName = (UserManager.Instance?.StudentData != null && UserManager.Instance.StudentData.ContainsKey("Nombre"))? UserManager.Instance.StudentData["Nombre"].ToString(): "Desconocido";

            // Actualizar el documento de la sala con los datos del estudiante
            foreach (DocumentSnapshot doc in task.Result.Documents)
            {
                UpdateRoomWithStudent(doc.Id, studentId, studentName);
            }
        }
        else
        {
            Debug.LogError("Código no encontrado.");
        }
    }

    private void UpdateRoomWithStudent(string roomId, string studentId, string studentName)
    {
        DocumentReference roomDocRef = db.Collection("rooms").Document(roomId);

        // Crear el objeto que representa al estudiante
        Dictionary<string, object> studentData = new Dictionary<string, object>
    {
        { "ID_Estudiante", studentId },
        { "Nombre", studentName }
    };

        // Actualizar el campo "Estudiantes" agregando el nuevo estudiante a la lista
        roomDocRef.UpdateAsync("Estudiantes", FieldValue.ArrayUnion(studentData)).ContinueWith(updateTask =>
        {
            if (updateTask.IsCompleted)
            {
                Debug.Log($"Estudiante {studentName} agregado a la sala con ID: {roomId}");
            }
            else
            {
                Debug.LogError("Error al agregar el estudiante a la sala: " + updateTask.Exception);
            }
        });
    }

    // Método para cambiar de escena
    public void ChangeScene()
    {
        Debug.Log("Cambiando a la escena: " + targetSceneName);
        SceneManager.LoadScene(targetSceneName); // Directamente, por ahora
    }
}