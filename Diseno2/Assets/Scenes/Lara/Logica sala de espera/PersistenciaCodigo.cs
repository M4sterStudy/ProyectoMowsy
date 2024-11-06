using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PersistenciaCodigo : MonoBehaviour
{
    public static PersistenciaCodigo Instance { get; private set; } // Singleton para acceso global
    [SerializeField] private TMP_InputField inputField; // Campo de texto de donde se extrae la información
    private string codigoGuardado; // Variable privada que almacena el código ingresado

    private void Awake()
    {
        // Implementar el patrón Singleton
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

    // Método para guardar el código desde el campo de texto
    public void GuardarCodigo()
    {
        if (inputField != null)
        {
            codigoGuardado = inputField.text;
            Debug.Log("Código guardado: " + codigoGuardado);
        }
        else
        {
            Debug.LogWarning("El campo de texto no está asignado en PersistenciaCodigo.");
        }
    }

    // Método público para que otros scripts puedan obtener el código guardado
    public string ObtenerCodigo()
    {
        return codigoGuardado;
    }
}
