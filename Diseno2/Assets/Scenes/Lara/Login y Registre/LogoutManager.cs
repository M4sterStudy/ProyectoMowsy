using UnityEngine;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement; // Asegúrate de incluir esto

public class LogoutManager : MonoBehaviour
{
    private FirebaseAuth auth;

    [SerializeField] private string loginSceneName; // Nombre de la escena de inicio de sesión
    [SerializeField] private TMP_Text feedbackText; // Referencia al mensaje de retroalimentación para el usuario

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;

        if (feedbackText == null)
        {
            Debug.LogError("Feedback text component not assigned.");
        }
    }

    public void Logout()
    {
        if (auth != null)
        {
            try
            {
                auth.SignOut();
                Debug.Log("User logged out successfully.");

                // Actualizar el estado del usuario
                UserManager.Instance.ClearCurrentUser();

                // Cambiar a la escena de inicio de sesión usando SceneManager directamente
                SceneManager.LoadScene(loginSceneName); // Carga la escena de inicio de sesión
            }
            catch (System.Exception)
            {
                ShowUserMessage("Ocurrió un problema al cerrar la sesión. Por favor, intenta nuevamente.");
            }
        }
        else
        {
            ShowUserMessage("No se pudo cerrar la sesión en este momento. Por favor, verifica tu conexión a Internet e inténtalo de nuevo.");
        }
    }

    private void ShowUserMessage(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.color = Color.red; // Cambia el color a rojo para los mensajes de retroalimentación de error
            feedbackText.text = message;
        }
        else
        {
            Debug.LogError("Feedback text component not assigned.");
        }
    }
}
