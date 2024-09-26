using System.Collections;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.SearchService;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEditor.PlayerSettings;

public class SceneChanger : MonoBehaviour
{
//    Cómo usar el script:
//Crear un objeto vacío:

//En Unity, haz clic derecho en la jerarquía y selecciona Crear > Objeto vacío.
//Asigna un nombre al objeto vacío, por ejemplo, SceneManager.
//Asignar el script al objeto vacío:

//Arrastra y suelta el script SceneChanger en el objeto vacío que creaste.
//Configurar el script desde el inspector:

//Verás en el inspector dos listas: Scene Names y Buttons.
//Scene Names: Aquí debes añadir los nombres exactos de las escenas que quieres que se cambien (como están registradas en el "Build Settings" de Unity).
//Buttons: Arrastra y suelta los botones que tienes en la escena actual.Estos serán los botones que activarán el cambio de escena.
//Asegurarse de que las escenas estén en el "Build Settings":

//Ve a File > Build Settings y asegúrate de que todas las escenas a las que quieras cambiar estén añadidas en la lista del "Build Settings".
//    // Listas serializadas para configurar desde el inspector


    [SerializeField] private List<string> sceneNames;   // Lista para almacenar los nombres de las escenas
    [SerializeField] private List<Button> buttons;      // Lista para almacenar los botones correspondientes a cada escena

    // Método para asignar los listeners de los botones
    private void Start()
    {
        // Verifica que la cantidad de escenas coincida con la cantidad de botones
        if (sceneNames.Count != buttons.Count)
        {
            Debug.LogError("La cantidad de escenas no coincide con la cantidad de botones.");
            return;
        }

        // Asigna los listeners a cada botón
        for (int i = 0; i < buttons.Count; i++)
        {
            int index = i;  // Necesario para evitar problemas de captura de variables en los closures
            buttons[index].onClick.AddListener(() => ChangeScene(sceneNames[index]));
        }
    }

    // Método para cambiar de escena
    private void ChangeScene(string sceneName)
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            // Carga la escena correspondiente
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("El nombre de la escena está vacío.");
        }
    }
}
