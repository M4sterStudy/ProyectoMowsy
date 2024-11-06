using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PersistenciaCodigo : MonoBehaviour
{
    public static PersistenciaCodigo Instance { get; private set; } // Singleton para acceso global
    [SerializeField] private TMP_InputField inputField; // Campo de texto de donde se extrae la informaci�n
    private string codigoGuardado; // Variable privada que almacena el c�digo ingresado

    private void Awake()
    {
        // Implementar el patr�n Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Hacer persistente el objeto
        }
        else
        {
            Destroy(gameObject); // Evitar duplicados
        }
    }

    // M�todo para guardar el c�digo desde el campo de texto
    public void GuardarCodigo()
    {
        if (inputField != null)
        {
            codigoGuardado = inputField.text;
            Debug.Log("C�digo guardado: " + codigoGuardado);
        }
        else
        {
            Debug.LogWarning("El campo de texto no est� asignado en PersistenciaCodigo.");
        }
    }

    // M�todo p�blico para que otros scripts puedan obtener el c�digo guardado
    public string ObtenerCodigo()
    {
        return codigoGuardado;
    }
}
