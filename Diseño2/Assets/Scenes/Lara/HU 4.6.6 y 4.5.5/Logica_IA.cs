using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

public class Logica_IA : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown topicDropdown;  // Dropdown de TextMeshPro
    [SerializeField] private TMP_Text textContent;  // Texto de TextMeshPro
    [SerializeField] private TMP_InputField summaryInput;  // InputField de TextMeshPro
    [SerializeField] private TMP_Text resultText;  // Texto para mostrar el resultado con TextMeshPro
    [SerializeField] private TMP_Text resumenErroresText; 
    [SerializeField] private Button submitButton;

    // API Settings
    private string token = "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";
    private string model = "meta-llama/Meta-Llama-3-8B-Instruct";
    private string apiUrl = "https://api-inference.huggingface.co/models/meta-llama/Meta-Llama-3-8B-Instruct/v1/chat/completions";

    // Predefined texts for each topic
    private Dictionary<string, string> texts = new Dictionary<string, string>() {
        { "videojuego", "La Gamificación y Sus Aplicaciones en la Educación La gamificación, o la aplicación de elementos de juego en contextos no lúdicos, se ha convertido en una herramienta poderosa en el campo de la educación. Esta técnica utiliza aspectos como puntos, niveles, recompensas y desafíos para motivar a los estudiantes, haciendo que el aprendizaje sea más atractivo y efectivo. En la educación, la gamificación ha demostrado ser particularmente útil para aumentar la participación y el compromiso de los estudiantes. Al transformar tareas educativas en actividades de juego, los estudiantes están más inclinados a involucrarse activamente en su aprendizaje. Por ejemplo, plataformas educativas como Duolingo utilizan elementos de gamificación para enseñar idiomas, ofreciendo recompensas por el progreso y permitiendo a los usuarios competir con amigos. La gamificación también puede ayudar a personalizar la experiencia de aprendizaje. Al permitir que los estudiantes progresen a su propio ritmo y proporcionándoles retroalimentación inmediata, la gamificación puede adaptarse a las necesidades individuales de cada estudiante. Esto es especialmente beneficioso en entornos de aprendizaje mixto, donde los estudiantes pueden trabajar de manera autónoma en actividades gamificadas mientras reciben apoyo personalizado de los profesores. Además, la gamificación puede fomentar habilidades importantes como la resolución de problemas, el pensamiento crítico y la colaboración. Los juegos a menudo requieren que los jugadores superen desafíos complejos, trabajen en equipo y piensen estratégicamente, habilidades que son altamente valoradas en la educación y en el mundo laboral." },
        { "cine", "La Importancia de los Premios Cinematográficos Los premios cinematográficos, como los Premios Óscar, los Globos de Oro y el Festival de Cannes, juegan un papel fundamental en la industria del cine. No solo reconocen y celebran el talento y la excelencia en la realización cinematográfica, sino que también influyen en la carrera de los cineastas y en la percepción del público sobre las películas. Ganar un premio importante puede transformar la carrera de un actor, director o guionista, abriéndoles puertas a nuevas oportunidades y elevando su estatus en la industria. Además, los premios a menudo impulsan el éxito comercial de una película, atrayendo a más espectadores y aumentando la recaudación en taquilla. Los premios también tienen un impacto significativo en la visibilidad y reconocimiento de ciertas películas y temas. Películas que tratan temas difíciles o que provienen de cineastas menos conocidos a menudo reciben un impulso de visibilidad cuando son nominadas o ganan premios. Esto no solo beneficia a los creadores, sino que también enriquece la diversidad y profundidad del cine que llega a las audiencias globales. Sin embargo, los premios cinematográficos no están exentos de controversias. A lo largo de los años, ha habido críticas sobre la falta de diversidad en las nominaciones y ganadores, así como sobre el enfoque comercial de algunas ceremonias de premios. A pesar de estas críticas, los premios siguen siendo una parte integral de la industria cinematográfica y continúan moldeando la manera en que el cine es percibido y valorado." },
        { "literatura", "El Impacto de la Literatura Clásica en la Cultura Moderna La literatura clásica ha sido fundamental en la formación de la cultura occidental y sigue siendo una fuente de inspiración para la cultura moderna. Obras como \"La Odisea\" de Homero, \"Don Quijote\" de Miguel de Cervantes, y \"Hamlet\" de William Shakespeare no solo son pilares de la literatura mundial, sino que también han influido en una amplia gama de disciplinas, desde el cine y el teatro hasta la filosofía y la política. La relevancia de la literatura clásica radica en su capacidad para explorar temas universales que siguen resonando en la actualidad. La búsqueda del héroe, la lucha entre el bien y el mal, el amor y la traición, son algunos de los temas que estos textos abordan de manera profunda y compleja. Estos temas son tan relevantes hoy como lo fueron en la época en que estas obras fueron escritas, y continúan siendo explorados en la literatura, el cine y otros medios modernos. Además, la literatura clásica ha dejado una huella indeleble en el lenguaje y las formas narrativas que utilizamos hoy en día. Expresiones como \"ser o no ser\" o \"tilting at windmills\" provienen directamente de obras clásicas y han entrado en el vocabulario común. Las estructuras narrativas y los arquetipos de personajes que se encuentran en estas obras también han sido adoptados y adaptados por autores y creadores modernos, demostrando la perenne influencia de estos textos. Leer literatura clásica no solo ofrece una ventana a las ideas y valores de las épocas pasadas, sino que también enriquece nuestra comprensión del presente. Las lecciones y reflexiones contenidas en estas obras pueden proporcionar una perspectiva valiosa sobre los desafíos contemporáneos y ayudarnos a comprender mejor la naturaleza humana. Es por eso que, a pesar del paso del tiempo, la literatura clásica sigue siendo estudiada y apreciada en todo el mundo." }
    };

    // Pesos para cada criterio de evaluación (en proporción 0-1)
    private readonly Dictionary<string, float> criteriosPesos = new Dictionary<string, float>() {
    { "sintesis", 0.20f },
    { "reconstruccion", 0.20f },
    { "objetividad", 0.10f },
    { "cohesion", 0.20f },
    { "coherencia", 0.10f },
    { "marcastextuales", 0.10f },
    { "puntuacion", 0.10f }
    };

        // Updated dictionary for error colors and weights
    private readonly Dictionary<string, (string color, float weight)> errorInfo = new Dictionary<string, (string, float)>
    {
        {"SINTESIS", ("#0000FF", 0.20f)},      // Azul
        {"RECONSTRUCCION", ("#00FF00", 0.20f)},// Verde
        {"OBJETIVIDAD", ("#FFFF00", 0.10f)},   // Amarillo
        {"COHESION", ("#FFA500", 0.20f)},      // Naranja
        {"COHERENCIA", ("#FF0000", 0.10f)},    // Rojo
        {"MARCAS", ("#FF0080", 0.10f)},        // Rosa
        {"PUNTUACION", ("#800080", 0.10f)}     // Morado
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
            StartCoroutine(MarkErrorsInSummary(selectedTopic, summary));
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

        string prompt = $@"
        Califica el siguiente resumen del texto relacionado con {topic}. 
        Utiliza los siguientes criterios, asignando una calificación de 0 a 5 para cada uno:
        1. Síntesis adecuada del sentido del texto base 
        2. Reconstrucción precisa de la idea principal y las ideas de apoyo 
        3. Objetividad en la información presentada (evitar opiniones)
        4. Cohesión del resumen (uso adecuado de referencias, omisiones, sinónimos, generalizaciones y conectores)
        5. Coherencia de las ideas presentadas
        6. Uso de marcas textuales que expliciten la voz del autor del texto base
        7. Uso adecuado de los signos de puntuación

        IMPORTANTE: Responde SOLAMENTE en español, utilizando EXACTAMENTE el siguiente formato sin agregar ningún texto adicional:

        Síntesis: X/5
        Reconstrucción: X/5
        Objetividad: X/5
        Cohesión: X/5
        Coherencia: X/5
        Marcas Textuales: X/5
        Puntuación: X/5

        Resumen a evaluar: ""{summary}""
        ";

        // Eliminar cualquier historial, enviando solo el prompt actual
        JObject payload = new JObject(
            new JProperty("model", model),
            new JProperty("messages", new JArray(
                new JObject(
                    new JProperty("role", "user"),
                    new JProperty("content", prompt)
                )
            )),
            new JProperty("max_tokens", 500),
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
                JObject jsonResponse = JObject.Parse(request.downloadHandler.text);
                string message = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString() ?? "No response";

                // Procesar el mensaje de respuesta y extraer los puntajes de cada criterio
                Dictionary<string, float> calificaciones = ProcesarCalificaciones(message);

                // Calcular la calificación final ponderada
                float calificacionFinal = CalcularCalificacionFinal(calificaciones);


                string cleanedResponse = LimpiarYExtraerRespuestaAPI(message);



                // Mostrar el resultado con colores
                string resultadoColoreado = AplicarColores(message);
                resultText.text = $"Calificación Final: {calificacionFinal:F2}/5\n{resultadoColoreado}";
            }
            else
            {
                resultText.text = $"Error: {request.error}";
            }
        }
    }


    // Procesar el texto de respuesta y extraer las calificaciones para cada criterio
    Dictionary<string, float> ProcesarCalificaciones(string message)
    {
        Dictionary<string, float> calificaciones = new Dictionary<string, float>();
        string[] lineas = message.Split('\n');

        foreach (string linea in lineas)
        {
            if (linea.Contains(":"))
            {
                string[] partes = linea.Split(':');
                string criterioOriginal = partes[0].Trim();
                string criterioNormalizado = NormalizarCriterio(criterioOriginal);

                if (criteriosPesos.ContainsKey(criterioNormalizado))
                {
                    if (float.TryParse(partes[1].Trim().Split('/')[0], out float calificacion))
                    {
                        calificaciones[criterioNormalizado] = calificacion;
                    }
                    else
                    {
                        Debug.LogWarning($"No se pudo parsear la calificación para {criterioNormalizado}: {partes[1]}");
                    }
                }
            }
        }

        return calificaciones;
    }

    private string NormalizarCriterio(string criterio)
    {
        // Eliminar tildes y convertir a minúsculas
        string normalizado = criterio.Normalize(System.Text.NormalizationForm.FormD);
        var sinTildes = new System.Text.StringBuilder();
        foreach (char c in normalizado)
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                sinTildes.Append(c);
            }
        }

        // Convertir a minúsculas, quitar espacios y caracteres especiales
        return Regex.Replace(sinTildes.ToString().ToLower(), @"[^a-z0-9]", "");
    }

    // Calcular la calificación final ponderada
    float CalcularCalificacionFinal(Dictionary<string, float> calificaciones)
    {
        float calificacionFinal = 0f;
        Debug.Log("Calculando calificación final:");
        foreach (var criterio in criteriosPesos)
        {
            if (calificaciones.TryGetValue(criterio.Key, out float calificacion))
            {
                float contribucion = calificacion * criterio.Value;
                calificacionFinal += contribucion;
                Debug.Log($"{criterio.Key}: calificación = {calificacion}, peso = {criterio.Value}, contribución = {contribucion}");
            }
            else
            {
                Debug.LogWarning($"No se encontró calificación para el criterio: {criterio.Key}");
            }
        }
        Debug.Log($"Calificación final calculada: {calificacionFinal}");
        return calificacionFinal;
    }

    // Aplica los colores a cada criterio en el mensaje de evaluación
    string AplicarColores(string message)
    {
        string[] lineas = message.Split('\n');  // Separa el texto por líneas
        string resultadoColoreado = "";

        // Crear un diccionario para mapear los nombres de los criterios con los colores
        Dictionary<string, string> criteriosColoresMap = new Dictionary<string, string>() {
        { "Síntesis", "#0000FF" },  // Azul
        { "Reconstrucción", "#00FF00" },  // Verde
        { "Objetividad", "#FFFF00" },  // Amarillo
        { "Cohesión", "#FFA500" },  // Naranja
        { "Coherencia", "#FF0000" },  // Rojo
        { "Marcas Textuales", "#ff0080" },  // Rosa
        { "Puntuación", "#800080" }  // Morado
    };

        foreach (string linea in lineas)
        {
            if (linea.Contains(":"))  // Asegurarse de que sea una línea con formato de criterio
            {
                string[] partes = linea.Split(':');
                string criterio = partes[0].Trim();  // Eliminar espacios alrededor del nombre del criterio
                string calificacion = partes.Length > 1 ? partes[1].Trim() : "";

                // Verifica si el criterio está en el diccionario de colores
                if (criteriosColoresMap.ContainsKey(criterio))
                {
                    string colorHex = criteriosColoresMap[criterio];
                    resultadoColoreado += $"<color={colorHex}>{partes[0]}: {calificacion}</color>\n";
                }
                else
                {
                    // Si no se encuentra el criterio, se muestra sin color
                    resultadoColoreado += $"{linea}\n";
                }
            }
            else
            {
                // Agrega cualquier línea que no sea un criterio (ejemplo, resumen) sin modificar
                resultadoColoreado += $"{linea}\n";
            }
        }

        return resultadoColoreado;
    }

    // New method to send the summary for error marking
    IEnumerator MarkErrorsInSummary(string topic, string summary)
    {
        resumenErroresText.text = "procesando...";

        string prompt = $@"
        INSTRUCCIONES CRÍTICAS: REALIZA UN ANÁLISIS EXHAUSTIVO Y EXTREMADAMENTE AGRESIVO DEL SIGUIENTE RESUMEN. MARCA TODOS LOS ERRORES POSIBLES, INCLUSO SI SON MENORES, SUTILES O DUDOSOS. UTILIZA MÚLTIPLES ETIQUETAS EN LA MISMA FRASE O PALABRA SI ES NECESARIO.

        Analiza minuciosamente el siguiente resumen del texto relacionado con {topic} y marca ABSOLUTAMENTE TODOS los errores específicos utilizando las siguientes etiquetas:
        [SINTESIS] para cualquier falla, por mínima que sea, en la síntesis adecuada del sentido del texto base
        [RECONSTRUCCION] para cualquier imprecisión, incluso leve, en la reconstrucción de la idea principal y las ideas de apoyo
        [OBJETIVIDAD] para cualquier indicio, por sutil que sea, de falta de objetividad o presencia de opiniones personales
        [COHESION] para cualquier error en la cohesión del resumen (uso inadecuado de referencias, omisiones, sinónimos, generalizaciones y conectores)
        [COHERENCIA] para cualquier inconsistencia o falta de lógica en las ideas presentadas, sin importar cuán pequeña sea
        [MARCAS] para cualquier ausencia de marcas textuales que expliciten la voz del autor del texto base
        [PUNTUACION] para cualquier uso inadecuado de los signos de puntuación, incluso si es un error muy menor

        IMPORTANTE:
        1. Inserta las etiquetas directamente en el texto del resumen, justo antes Y después de las partes donde se cometen los errores.
        2. Usa MÚLTIPLES ETIQUETAS en la misma palabra o frase si detectas varios tipos de errores.
        3. Sé EXTREMADAMENTE CRÍTICO Y AGRESIVO. Marca incluso los errores más sutiles, dudosos o mínimos.
        4. NO OMITAS ABSOLUTAMENTE NINGÚN ERROR POTENCIAL, por pequeño que sea.
        5. NO AGREGUES NINGÚN TEXTO ADICIONAL O EXPLICACIÓN. SOLO DEVUELVE EL RESUMEN ORIGINAL CON LAS ETIQUETAS INSERTADAS.

        Resumen: ""{summary}""
        ";

        JObject payload = new JObject(
            new JProperty("model", model),
            new JProperty("messages", new JArray(
                new JObject(
                    new JProperty("role", "user"),
                    new JProperty("content", prompt)
                )
            )),
            new JProperty("max_tokens", 1000),
            new JProperty("stream", false)
        );

        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(apiUrl, ""))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(payload.ToString());
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Authorization", $"Bearer {token}");
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                JObject jsonResponse = JObject.Parse(request.downloadHandler.text);
                string aiResponse = jsonResponse["choices"]?[0]?["message"]?["content"]?.ToString() ?? "No response";

                // Log the complete AI response
                Debug.Log("Respuesta completa de la IA para la marcación de errores: " + aiResponse);

                // Clean the AI response and extract only the marked summary
                string cleanedMarkedSummary = LimpiarYExtraerRespuestaAPI(aiResponse, summary);

                // Log the cleaned marked summary
                Debug.Log("Resumen marcado y limpiado: " + cleanedMarkedSummary);

                // Process and display the marked summary
                string coloredSummary = ProcessMarkedSummary(cleanedMarkedSummary);
                resumenErroresText.text = coloredSummary;
            }
            else
            {
                Debug.LogError($"Error marking errors: {request.error}");
                resumenErroresText.text = "Error al marcar los errores en el resumen.";
            }
        }
    }

    // Method to process the marked summary and apply colors
    string ProcessMarkedSummary(string markedSummary)
    {
        StringBuilder processedSummary = new StringBuilder();
        Stack<string> openTags = new Stack<string>();
        int i = 0;

        while (i < markedSummary.Length)
        {
            if (markedSummary[i] == '[')
            {
                int closeBracketIndex = markedSummary.IndexOf(']', i);
                if (closeBracketIndex != -1)
                {
                    string tag = markedSummary.Substring(i + 1, closeBracketIndex - i - 1);
                    if (errorInfo.ContainsKey(tag))
                    {
                        string color = errorInfo[tag].color;
                        processedSummary.Append($"<color={color}>");
                        openTags.Push(tag);
                    }
                    i = closeBracketIndex + 1;
                }
                else
                {
                    processedSummary.Append(markedSummary[i]);
                    i++;
                }
            }
            else if (markedSummary[i] == ']' && openTags.Count > 0)
            {
                processedSummary.Append("</color>");
                openTags.Pop();
                i++;
            }
            else
            {
                processedSummary.Append(markedSummary[i]);
                i++;
            }
        }

        // Close any remaining open tags
        while (openTags.Count > 0)
        {
            processedSummary.Append("</color>");
            openTags.Pop();
        }

        return processedSummary.ToString();
    }
    // Updated method to clean and extract API response
    private string LimpiarYExtraerRespuestaAPI(string respuestaAPI, string resumenOriginal = null)
    {
        // Remove any text before or after quotes
        Match match = Regex.Match(respuestaAPI, @"""(.+?)""", RegexOptions.Singleline);
        if (match.Success)
        {
            respuestaAPI = match.Groups[1].Value;
        }

        // Remove any additional quotes at the beginning or end
        respuestaAPI = respuestaAPI.Trim('"');

        if (resumenOriginal != null)
        {
            // Clean the response based on the original summary
            StringBuilder respuestaLimpia = new StringBuilder();
            int indiceOriginal = 0;
            int i = 0;

            while (i < respuestaAPI.Length)
            {
                if (respuestaAPI[i] == '[')
                {
                    int indiceCierreBracket = respuestaAPI.IndexOf(']', i);
                    if (indiceCierreBracket != -1)
                    {
                        respuestaLimpia.Append(respuestaAPI.Substring(i, indiceCierreBracket - i + 1));
                        i = indiceCierreBracket + 1;
                    }
                    else
                    {
                        i++;
                    }
                }
                else if (indiceOriginal < resumenOriginal.Length &&
                         (respuestaAPI[i] == resumenOriginal[indiceOriginal] ||
                          char.ToLower(respuestaAPI[i]) == char.ToLower(resumenOriginal[indiceOriginal])))
                {
                    respuestaLimpia.Append(resumenOriginal[indiceOriginal]);
                    indiceOriginal++;
                    i++;
                }
                else
                {
                    i++;
                }
            }

            // Add any remaining text from the original summary
            if (indiceOriginal < resumenOriginal.Length)
            {
                respuestaLimpia.Append(resumenOriginal.Substring(indiceOriginal));
            }

            return respuestaLimpia.ToString();
        }

        return respuestaAPI;
    }


}
