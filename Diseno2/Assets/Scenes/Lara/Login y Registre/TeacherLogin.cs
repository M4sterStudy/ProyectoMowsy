using UnityEngine;
using Firebase;
using Firebase.Auth;
using Firebase.Firestore;
using System.Collections.Generic;
using TMPro;
using System.Threading.Tasks;
using System.Collections;

public class TeacherLogin : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;
    private bool isFirebaseInitialized;
    private FirebaseInitializer firebaseInitializer;
    private SceneLoader sceneLoader;

    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TMP_Text errorText;

    void Start()
    {
        firebaseInitializer = FindObjectOfType<FirebaseInitializer>();
        if (firebaseInitializer == null)
        {
            Debug.LogError("FirebaseInitializer not found in the scene.");
            return;
        }

        StartCoroutine(WaitForFirebaseInitialization());

        if (sceneLoader == null)
        {
            sceneLoader = FindObjectOfType<SceneLoader>();
            if (sceneLoader == null)
            {
                Debug.LogError("SceneLoader not found in the scene.");
            }
        }
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
        isFirebaseInitialized = true;
        Debug.Log("Firebase Auth and Db references set in TeacherLogin");

        if (auth == null)
        {
            Debug.LogError("Firebase Auth is still null after initialization in TeacherLogin");
        }
        else
        {
            Debug.Log("Firebase Auth initialized successfully in TeacherLogin");
        }
    }

    public void LoginTeacher()
    {
        Debug.Log("LoginTeacher method called");

        if (!isFirebaseInitialized || auth == null)
        {
            ShowError("Firebase no está inicializado aún. Por favor, espere.");
            return;
        }

        string email = emailInput.text.Trim();
        string password = passwordInput.text.Trim();

        Debug.Log($"Attempting to login teacher with email: {email}");

        if (!ValidateInputs(email, password))
        {
            return;
        }

        Debug.Log("Input validation passed, signing in user...");

        auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWith(task => {
            if (task.IsCanceled)
            {
                ShowError("El inicio de sesión fue cancelado.");
                return;
            }
            if (task.IsFaulted)
            {
                HandleAuthError(task.Exception);
                return;
            }

            // Authentication successful
            FirebaseUser user = task.Result.User;
            UserManager.Instance.SetCurrentUser(user); // Guarda el usuario en UserManager
            Debug.Log($"Teacher signed in successfully: {user.Email}");
            FetchTeacherData(user.UserId);
            FetchAndLogTeacherData(user.UserId); // Para consultar los datos del profesor que ingresa. 
            ShowSuccess("Inicio de sesión exitoso. ¡Bienvenido!");
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    private bool ValidateInputs(string email, string password)
    {
        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
        {
            ShowError("El correo electrónico y la contraseña son obligatorios.");
            return false;
        }

        if (!System.Text.RegularExpressions.Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            ShowError("El formato del correo electrónico no es válido.");
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
            else
            {
                Debug.LogError($"Unexpected error type: {innerException.GetType()}");
                ShowError("Se produjo un error inesperado durante el inicio de sesión.");
            }
        }
    }

    private async Task FetchTeacherData(string userId)
    {
        try
        {
            Debug.Log($"Fetching teacher data for user: {userId}");

            DocumentReference teacherRef = db.Collection("Profesores").Document(userId);
            DocumentSnapshot snapshot = await teacherRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> teacherData = snapshot.ToDictionary();
                UserManager.Instance.SetTeacherData(teacherData); // Guarda los datos en UserManager

                if (teacherData.TryGetValue("Rol", out object rol) && rol.ToString() == "Profesor")
                {
                    Debug.Log("Usuario verificado como profesor");
                    OnSuccessfulLogin("Profesor");
                }
                else
                {
                    Debug.LogWarning("El usuario no es un profesor");
                    ShowError("Acceso denegado. Esta cuenta no pertenece a un profesor.");
                }
            }
            else
            {
                ShowError("No se encontraron datos de profesor para este usuario.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error fetching teacher data: {ex.Message}");
            ShowError("Se produjo un error al obtener los datos del profesor.");
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


    private void OnSuccessfulLogin(string role)
    {
        ShowSuccess("Inicio de sesión exitoso. ¡Bienvenido!");
        StartCoroutine(DelayedSceneChange(role));
    }

    private System.Collections.IEnumerator DelayedSceneChange(string role)
    {
        yield return new WaitForSeconds(1.5f);

        if (sceneLoader != null)
        {
            sceneLoader.LoadNextScene(role);
        }
        else
        {
            Debug.LogError("SceneLoader is not assigned. Cannot change scene.");
        }
    }

    private string GetFirebaseErrorMessage(Firebase.Auth.AuthError errorCode)
    {
        switch (errorCode)
        {
            case AuthError.MissingEmail:
                return "Por favor, ingrese un correo electrónico.";
            case AuthError.InvalidEmail:
                return "El formato del correo electrónico no es válido.";
            case AuthError.WrongPassword:
                return "La contraseña es incorrecta. Por favor, inténtelo de nuevo.";
            case AuthError.UserNotFound:
                return "No existe una cuenta asociada a este correo electrónico. Por favor, verifique o regístrese.";
            case AuthError.UserDisabled:
                return "Esta cuenta ha sido deshabilitada. Contacte al soporte para más información.";
            case AuthError.EmailAlreadyInUse:
                return "Este correo electrónico ya está en uso. ¿Olvidó su contraseña?";
            case AuthError.WeakPassword:
                return "La contraseña es demasiado débil. Debe tener al menos 6 caracteres.";
            case AuthError.OperationNotAllowed:
                return "Esta operación no está permitida. Contacte al soporte si cree que esto es un error.";
            default:
                return "Se produjo un error inesperado durante el inicio de sesión. Por favor, inténtelo de nuevo.";
        }
    }

    private async Task FetchAndLogTeacherData(string userId)
    {
        try
        {
            Debug.Log($"Fetching teacher data for user: {userId}");

            DocumentReference teacherRef = db.Collection("Profesores").Document(userId);
            DocumentSnapshot snapshot = await teacherRef.GetSnapshotAsync();

            if (snapshot.Exists)
            {
                Dictionary<string, object> teacherData = snapshot.ToDictionary();

                string nombre = teacherData.ContainsKey("Nombre") ? teacherData["Nombre"].ToString() : "Desconocido";
                string apellido = teacherData.ContainsKey("Apellido") ? teacherData["Apellido"].ToString() : "Desconocido";
                string usuario = teacherData.ContainsKey("Usuario") ? teacherData["Usuario"].ToString() : "Desconocido";
                string correo = teacherData.ContainsKey("Correo") ? teacherData["Correo"].ToString() : "Desconocido";
                string rol = teacherData.ContainsKey("Rol") ? teacherData["Rol"].ToString() : "Desconocido";

                Debug.Log($"Datos del profesor:\nNombre: {nombre}\nApellido: {apellido}\nUsuario: {usuario}\nCorreo: {correo}\nRol: {rol}");
            }
            else
            {
                Debug.LogWarning($"No se encontraron datos del profesor para el usuario: {userId}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Error al recuperar datos del profesor: {ex.Message}");
        }
    }

}