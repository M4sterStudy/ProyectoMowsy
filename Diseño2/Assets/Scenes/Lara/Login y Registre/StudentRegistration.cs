using Firebase.Auth;
using Firebase;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Firebase.Firestore;
using System.Threading.Tasks;

public class StudentRegistration : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private bool isFirebaseInitialized;
    private FirebaseInitializer firebaseInitializer;

    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField lastNameInput;
    [SerializeField] private TMP_Text errorText;

    void Start()
    {
        Debug.Log("StudentRegistration Start method called");
        firebaseInitializer = FindObjectOfType<FirebaseInitializer>();
        if (firebaseInitializer == null)
        {
            Debug.LogError("FirebaseInitializer not found in the scene.");
            return;
        }

        Debug.Log($"FirebaseInitializer found. IsInitialized: {firebaseInitializer.IsInitialized}");

        StartCoroutine(WaitForFirebaseInitialization());

        if (!firebaseInitializer.IsInitialized)
        {
            Debug.LogWarning("Firebase no está inicializado aún. Por favor, espere.");
            return;
        }

        auth = firebaseInitializer.Auth;
        db = firebaseInitializer.Db;
        Debug.Log("Firebase Auth and Db references set");
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

    public void RegisterStudent()
    {
        Debug.Log("RegisterStudent method called");

        if (auth == null)
        {
            Debug.LogError("Firebase Auth is null. Cannot register user.");
            ShowError("Error de inicialización. Por favor, intente de nuevo más tarde.");
            return;
        }

        if (firebaseInitializer == null)
        {
            Debug.LogError("FirebaseInitializer is null");
            return;
        }

        if (!firebaseInitializer.IsInitialized)
        {
            Debug.LogError("Firebase no está inicializado aún. Por favor, espere.");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();
        string name = nameInput.text.Trim();
        string lastName = lastNameInput.text.Trim();

        Debug.Log($"Attempting to register user with email: {email}");

        if (!ValidateInputs(email, password, name, lastName))
        {
            Debug.LogError("Input validation failed");
            return;
        }

        Debug.Log("Input validation passed, creating user...");

        string username = GenerateUsername(name, lastName);
        Debug.Log($"Generated username: {username}");

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

            // Mostrar mensaje de éxito por la creación del usuario
            ShowSuccess("Usuario creado exitosamente.");


            CreateStudentDocument(newUser.UserId, newUser.Email, name, lastName, username);
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private string GenerateUsername(string name, string lastName)
    {
        string randomNumber = Random.Range(1000, 9999).ToString();
        string username = name.ToLower() + "." + lastName.ToLower() + randomNumber;
        Debug.Log("Generated username: " + username);
        return username;
    }

    private void CreateStudentDocument(string userId, string email, string name, string lastName, string username)
    {
        Debug.Log($"Creating student document for user: {userId}");

        DocumentReference studentRef = db.Collection("Estudiantes").Document(userId);
        Dictionary<string, object> studentData = new Dictionary<string, object>
        {
            { "Nombre", name },
            { "Apellido", lastName },
            { "Estado", "Activo" },
            { "Usuario", username }, // Se usa el username generado automáticamente
            { "Correo", email },
            { "Rol", "Estudiante" } 
        };

        studentRef.SetAsync(studentData).ContinueWith(task => {
            if (task.IsCompleted)
            {
                Debug.Log("Estudiante registrado exitosamente en Firestore");

                // Mostrar mensaje de éxito después de registrar al estudiante en Firestore
                ShowSuccess("Estudiante registrado exitosamente.");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error al registrar estudiante en Firestore: " + task.Exception);
            }
            else if (task.IsCanceled)
            {
                Debug.LogError("La operación de registro en Firestore fue cancelada");
            }
        });
    }

    private bool ValidateInputs(string email, string password, string name, string lastName)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(name) || string.IsNullOrEmpty(lastName))
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
                var authError = (AuthError)firebaseEx.ErrorCode;  // Hacemos un cast explícito aquí
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
