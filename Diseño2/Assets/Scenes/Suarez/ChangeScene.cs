using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeScene : MonoBehaviour
{
    public string sceneToLoad; // Nombre de la escena a cargar
    public Button yourButton; // Referencia al botón

    void Start()
    {
        // Asegúrate de que el botón no sea nulo
        if (yourButton != null)
        {
            yourButton.onClick.AddListener(OnButtonClick);
        }
        else
        {
            Debug.LogError("El botón no está asignado en el inspector.");
        }
    }

    void OnButtonClick()
    {
        // Cambia a la escena especificada
        SceneManager.LoadScene(sceneToLoad);
    }
}
