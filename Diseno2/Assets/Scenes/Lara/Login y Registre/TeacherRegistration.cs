using Firebase.Auth;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase.Firestore;
using System.Threading.Tasks;

public class TeacherRegistration : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private bool isFirebaseInitialized;
    private FirebaseInitializer firebaseInitializer;

    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField lastNameInput;
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_Text errorText;

    void Start()
    {
        Debug.Log("TeacherRegistration Start method called");
        firebaseInitializer = FindObjectOfType<FirebaseInitializer>();
        if (firebaseInitializer == null)
        {
            Debug.LogError("FirebaseInitializer not found in the scene.");
            return;
        }

        StartCoroutine(WaitForFirebaseInitialization());
    }

    private IEnumerator WaitForFirebaseInitialization()
    {
        while (!firebaseInitializer.IsInitialized)
        {
            Debug.Log("Waiting for Firebase to initialize...");
            yield return new WaitForSeconds(0.5f);
        }

        auth = firebaseInitializer.Auth;
        db = firebaseInitializer.Db;
        Debug.Log("Firebase Auth and Db references set");

        if (auth == null)
        {
            Debug.LogError("Firebase Auth is still null after initialization");
        }
        else
        {
            Debug.Log("Firebase Auth initialized successfully");
        }
    }

    public void RegisterTeacher()
    {
        Debug.Log("RegisterTeacher method called");

        if (auth == null)
        {
            Debug.LogError("Firebase Auth is null. Cannot register user.");
            ShowError("Error de inicialización. Por favor, intente de nuevo más tarde.");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();
        string name = nameInput.text.Trim();
        string lastName = lastNameInput.text.Trim();
        string username = usernameInput.text.Trim();

        Debug.Log($"Attempting to register teacher with email: {email}");

        if (!ValidateInputs(email, password, name, lastName, username))
        {
            Debug.LogError("Input validation failed");
            return;
        }

        Debug.Log("Input validation passed, creating user...");

        auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("User creation was canceled");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError($"CreateUserWithEmailAndPasswordAsync encountered an error: {task.Exception}");
                HandleAuthError(task.Exception);
                return;
            }

            Debug.Log("User created successfully");
            AuthResult result = task.Result;
            FirebaseUser newUser = result.User;
            Debug.Log($"Firebase user created successfully: {newUser.Email}");

            ShowSuccess("Usuario creado exitosamente.");

            CreateTeacherDocument(newUser.UserId, name, lastName, username, email);
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private void CreateTeacherDocument(string userId, string name, string lastName, string username, string email)
    {
        Debug.Log($"Creating teacher document for user: {userId}");

        DocumentReference teacherRef = db.Collection("Profesores").Document(userId);
        Dictionary<string, object> teacherData = new Dictionary<string, object>
    {
        { "ID_Profesor", userId },  // Cambiado para usar "ID_Profesor" como clave
        { "Nombre", name },
        { "Apellido", lastName },
        { "Usuario", username },
        { "Correo", email },
        { "Rol", "Profesor" }
    };

        teacherRef.SetAsync(teacherData).ContinueWith(task => {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al registrar profesor en Firestore: " + task.Exception);
            }
            else if (task.IsCanceled)
            {
                Debug.LogError("La operación de registro en Firestore fue cancelada");
            }
            else
            {
                Debug.Log("Profesor registrado exitosamente en Firestore");
                ShowSuccess("Profesor registrado exitosamente.");
            }
        });
    }

    private bool ValidateInputs(string email, string password, string name, string lastName, string username)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(name) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(username))
        {
            ShowError("Todos los campos son obligatorios.");
            return false;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            ShowError("El email no es válido.");
            return false;
        }

        if (password.Length < 6)
        {
            ShowError("La contraseña debe tener al menos 6 caracteres.");
            return false;
        }

        return true;
    }

    private void HandleAuthError(System.AggregateException exception)
    {
        foreach (var innerException in exception.InnerExceptions)
        {
            Debug.LogError($"Authentication error: {innerException.Message}");
            if (innerException is FirebaseException firebaseEx)
            {
                var authError = (AuthError)firebaseEx.ErrorCode;
                Debug.LogError($"Firebase error code: {authError}");
                ShowError(GetFirebaseErrorMessage(authError));
            }
        }
    }

    private void ShowError(string message)
    {
        errorText.color = Color.red;
        errorText.text = message;
    }

    private void ShowSuccess(string message)
    {
        errorText.color = Color.green;
        errorText.text = message;
    }

    private string GetFirebaseErrorMessage(Firebase.Auth.AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                return "Falta el correo electrónico.";
            case AuthError.InvalidEmail:
                return "El correo electrónico no es válido.";
            case AuthError.WeakPassword:
                return "La contraseña es demasiado débil.";
            case AuthError.EmailAlreadyInUse:
                return "El correo electrónico ya está en uso.";
            default:
                return "Se produjo un error durante el registro.";
        }
    }
}