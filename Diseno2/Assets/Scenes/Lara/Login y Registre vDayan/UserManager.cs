using Firebase.Auth;
using System.Collections.Generic;
using UnityEngine;

public class UserManager : MonoBehaviour
{
    public static UserManager Instance { get; private set; }
    public FirebaseUser CurrentUser { get; private set; }
    public Dictionary<string, object> TeacherData { get; private set; }
    public Dictionary<string, object> StudentData { get; private set; } // Agregado para almacenar datos de estudiantes

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
        OnTeacherDataLoaded?.Invoke(); // Disparar evento cuando se carguen los datos del profesor
    }

    // Método para guardar datos de estudiantes
    public void SetStudentData(Dictionary<string, object> data) // Agregado para estudiantes
    {
        Debug.Log("SetStudentData method called");
        StudentData = data;
        OnStudentDataLoaded?.Invoke(); // Disparar evento cuando se carguen los datos del estudiante
    }
}
