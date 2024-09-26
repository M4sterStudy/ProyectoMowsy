using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System;

public class Conexion_IA : MonoBehaviour
{

    // UI elements (convertidos a privados y serializados)
    [SerializeField] private TMP_Dropdown topicDropdown;  // Dropdown de TextMeshPro
    [SerializeField] private TMP_Text textContent;  // Texto de TextMeshPro
    [SerializeField] private TMP_InputField summaryInput;  // InputField de TextMeshPro
    [SerializeField] private TMP_Text resultText;  // Texto para mostrar el resultado con TextMeshPro
    [SerializeField] private Button submitButton;

    // API Settings (convertidos a privados)
    private string token = "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";
    private string model = "meta-llama/Meta-Llama-3-8B-Instruct";
    private string apiUrl = "https://api-inference.huggingface.co/models/meta-llama/Meta-Llama-3-8B-Instruct/v1/chat/completions";

    // Predefined texts for each topic
    private Dictionary<string, string> texts = new Dictionary<string, string>() {
        { "videojuego", "En los videojuegos, los jugadores asumen el rol de personajes en un mundo virtual. Estos juegos pueden variar desde simples plataformas hasta complejas simulaciones 3D. Los jugadores completan misiones, resuelven acertijos y luchan contra enemigos para avanzar en la historia o en la clasificación." },
        { "cine", "El cine es una forma de arte que utiliza imágenes en movimiento para contar historias. Desde los primeros cortometrajes hasta las producciones modernas en 4K, el cine ha evolucionado para incluir una variedad de géneros y estilos. Las películas pueden ser narrativas, documentales o experimentales." },
        { "literatura", "La literatura incluye todas las formas de escritura, desde novelas y cuentos hasta poesía y ensayos. A través de los siglos, la literatura ha reflejado y moldeado la cultura y el pensamiento humano. Los grandes autores han influido en la forma en que entendemos el mundo a través de sus palabras." }
    };

    void Start()
    {
        // Bind event listener for Dropdown
        topicDropdown.onValueChanged.AddListener(delegate { LoadText(); });

        // Bind event listener for Submit button
        submitButton.onClick.AddListener(SubmitSummary);
    }

    // Load the corresponding text based on the selected topic
    void LoadText()
    {
        string selectedTopic = topicDropdown.options[topicDropdown.value].text.ToLower();

        if (texts.ContainsKey(selectedTopic))
        {
            textContent.text = texts[selectedTopic];
        }
        else
        {
            textContent.text = "Por favor, selecciona una temática.";
        }
    }

    // Submit the summary to the Hugging Face API
    void SubmitSummary()
    {
        string summary = summaryInput.text;
        string selectedTopic = topicDropdown.options[topicDropdown.value].text.ToLower();

        if (!string.IsNullOrEmpty(summary) && texts.ContainsKey(selectedTopic))
        {
            StartCoroutine(SendSummaryToAPI(selectedTopic, summary));
        }
        else
        {
            resultText.text = "Por favor, selecciona una temática y escribe un resumen.";
        }
    }

    // Coroutine to handle the API request
    IEnumerator SendSummaryToAPI(string topic, string summary)
    {
        resultText.text = "Calificando...";

        // Build the payload
        string prompt = $"Califica el siguiente resumen del texto relacionado con {topic}. Usa el siguiente formato para tu respuesta, sin explicaciones adicionales:\n\nOrtografía: X/10\nPuntuación: X/10\nCalidad del resumen: X/10\n\nCalificación final: X.X/5.0\n\nResumen: \"{summary}\"";
        JObject payload = new JObject(
            new JProperty("model", model),
            new JProperty("messages", new JArray(
                new JObject(
                    new JProperty("role", "user"),
                    new JProperty("content", prompt)
                )
            )),
            new JProperty("max_tokens", 150),
            new JProperty("stream", false)
        );

        // Create the UnityWebRequest
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(apiUrl, ""))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(payload.ToString());
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            request.SetRequestHeader("Content-Type", "application/json");

            // Send the request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Parse the response
                JObject jsonResponse = JObject.Parse(request.downloadHandler.text);
                string message = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString() ?? "No response";
                resultText.text = $"Calificación: {message}";
            }
            else
            {
                resultText.text = $"Error: {request.error}";
            }
        }
    }
}
