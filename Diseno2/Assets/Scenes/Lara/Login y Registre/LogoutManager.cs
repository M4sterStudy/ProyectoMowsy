using UnityEngine;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement; // Aseg�rate de incluir esto

public class LogoutManager : MonoBehaviour
{
    private FirebaseAuth auth;

    [SerializeField] private string loginSceneName; // Nombre de la escena de inicio de sesi�n
    [SerializeField] private TMP_Text feedbackText; // Referencia al mensaje de retroalimentaci�n para el usuario

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

                // Cambiar a la escena de inicio de sesi�n usando SceneManager directamente
                SceneManager.LoadScene(loginSceneName); // Carga la escena de inicio de sesi�n
            }
            catch (System.Exception)
            {
                ShowUserMessage("Ocurri� un problema al cerrar la sesi�n. Por favor, intenta nuevamente.");
            }
        }
        else
        {
            ShowUserMessage("No se pudo cerrar la sesi�n en este momento. Por favor, verifica tu conexi�n a Internet e int�ntalo de nuevo.");
        }
    }

    private void ShowUserMessage(string message)
    {
        if (feedbackText != null)
        {
            feedbackText.color = Color.red; // Cambia el color a rojo para los mensajes de retroalimentaci�n de error
            feedbackText.text = message;
        }
        else
        {
            Debug.LogError("Feedback text component not assigned.");
        }
    }
}
