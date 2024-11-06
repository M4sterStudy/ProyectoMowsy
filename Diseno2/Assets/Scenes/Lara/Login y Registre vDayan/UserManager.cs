using Firebase.Auth;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }
    public FirebaseUser CurrentUser { get; private set; }
    public Dictionary<string, object> TeacherData { get; private set; }
    public Dictionary<string, object> StudentData { get; private set; } // Agregado para almacenar datos de estudiantes

    public string TeacherDataID { get; private set; }
    public string StudentDataID { get; private set; }


    public event System.Action OnTeacherDataLoaded;
    public event System.Action OnStudentDataLoaded;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Para mantener el objeto entre escenas
        }
        else
        {
            Destroy(gameObject); // Para prevenir duplicados
        }
    }

    public void SetCurrentUser(FirebaseUser user)
    {
        CurrentUser = user;
    }

    public void ClearCurrentUser() // Agrega este método
    {
        CurrentUser = null;
        TeacherData = null;
        StudentData = null;
        Debug.Log("User and related data cleared.");
    }

    // Método para guardar datos de profesores
    public void SetTeacherData(Dictionary<string, object> data)
    {
        Debug.Log("SetTeacherData method called");
        TeacherData = data;
       
        // Asigna el ID del profesor desde el diccionario, asegurándose de que use la clave "ID_Profesor"
        if (data.ContainsKey("ID_Profesor"))
        {
            TeacherDataID = data["ID_Profesor"].ToString();
            Debug.Log($"ID_Profesor establecido en UserManager: {TeacherDataID}");
        }
        else
        {
            Debug.LogError("El diccionario de datos de profesor no contiene la clave 'ID_Profesor'.");
        }

        OnTeacherDataLoaded?.Invoke(); // Disparar evento cuando se carguen los datos del profesor
    }


    // Método para guardar datos de estudiantes
    public void SetStudentData(Dictionary<string, object> data) // Método para cargar datos del estudiante
    {
        Debug.Log("SetStudentData method called");
        StudentData = data;

        if (data.ContainsKey("ID_Estudiante"))
        {
            StudentDataID = data["ID_Estudiante"].ToString();
            Debug.Log($"ID_Estudiante establecido en UserManager: {StudentDataID}");
        }
        else
        {
            Debug.LogError("El diccionario de datos de estudiante no contiene la clave 'ID_Estudiante'.");
        }

        OnStudentDataLoaded?.Invoke(); // Disparar evento cuando se carguen los datos del estudiante
    }
}
