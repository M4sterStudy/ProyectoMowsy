using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; // Necesario para trabajar con TMP_InputField


public class MantenerTexto : MonoBehaviour
{
    public TMP_InputField inputField; // Referencia al TMP_InputField

    // Variable estática para almacenar el texto del TMP_InputField
    private static string savedText = "";

    private void Awake()
    {
        // Asegúrate de que este objeto no sea destruido al cambiar de escena
        DontDestroyOnLoad(gameObject);
        
        // Verifica si ya existe una instancia de este objeto
        if (FindObjectsOfType<MantenerTexto>().Length > 1)
        {
            Destroy(gameObject); // Si ya existe otro, destruye este duplicado
        }
    }

    private void Start()
    {
        // Si hay un texto guardado, cargarlo en el TMP_InputField
        inputField.text = savedText;
    }

    private void OnDisable()
    {
        // Guardar el texto cuando la escena se deshabilita (al cambiar de escena)
        savedText = inputField.text;
    }
}