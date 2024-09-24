using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MensajeAleatorio : MonoBehaviour
{
    public GameObject[] mensajesPrefabs;
    public float intervaloMensajes = 2.0f;
    public float duracionTotal = 120.0f;
    public Transform canvasTransform;
    public float duracionMensaje = 10.0f;
    public float margenDerecho = 50.0f;
    public float margenVertical = 50.0f;
    public float velocidadDesplazamiento = 200f;
    public float duracionDesvanecimiento = 2.0f;

    private float tiempoTranscurrido = 0.0f;
    
    // Diccionario para llevar la cuenta de cuántas veces ha salido cada mensaje
    private Dictionary<GameObject, int> contadorMensajes = new Dictionary<GameObject, int>();

    void Start()
    {
        // Inicializamos el contador de cada mensaje
        foreach (GameObject mensaje in mensajesPrefabs)
        {
            contadorMensajes[mensaje] = 0; // Todos los mensajes empiezan sin haber salido
        }

        StartCoroutine(GenerarMensajesAleatoriamente());
    }

    IEnumerator GenerarMensajesAleatoriamente()
    {
        while (tiempoTranscurrido < duracionTotal)
        {
            // Escoger un mensaje priorizando los que han salido menos veces
            GameObject mensajeAleatorio = ElegirMensajePonderado();

            // Instanciar el mensaje en el Canvas
            GameObject mensajeInstanciado = Instantiate(mensajeAleatorio, canvasTransform);

            // Colocamos el mensaje en la parte superior derecha con márgenes ajustados
            RectTransform mensajeRect = mensajeInstanciado.GetComponent<RectTransform>();
            mensajeRect.anchoredPosition = new Vector2(Screen.width / 2 - margenDerecho, Screen.height / 2 - margenVertical);

            // Comenzar la animación de desplazamiento hacia la izquierda
            StartCoroutine(DesplazarMensaje(mensajeRect));

            // Esperar antes de destruir el mensaje (tiempo de vida)
            yield return new WaitForSeconds(duracionMensaje - duracionDesvanecimiento);

            // Comenzar la animación de desvanecimiento antes de destruir el mensaje
            CanvasGroup canvasGroup = mensajeInstanciado.GetComponent<CanvasGroup>();
            StartCoroutine(DesvanecerMensaje(canvasGroup));

            // Destruir el mensaje después de que el desvanecimiento haya terminado
            Destroy(mensajeInstanciado, duracionDesvanecimiento);

            // Aumentar el contador de apariciones de ese mensaje
            contadorMensajes[mensajeAleatorio]++;

            yield return new WaitForSeconds(intervaloMensajes);
            tiempoTranscurrido += intervaloMensajes;
        }
    }

    // Método para elegir un mensaje ponderando los que han salido menos veces
    GameObject ElegirMensajePonderado()
    {
        int totalApariciones = 0;
        foreach (int count in contadorMensajes.Values)
        {
            totalApariciones += count;
        }

        List<float> probabilidades = new List<float>();

        foreach (GameObject mensaje in mensajesPrefabs)
        {
            int apariciones = contadorMensajes[mensaje];
            float probabilidad = 1.0f / (1 + apariciones);
            probabilidades.Add(probabilidad);
        }

        float sumaTotal = 0f;
        foreach (float probabilidad in probabilidades)
        {
            sumaTotal += probabilidad;
        }

        float valorAleatorio = Random.Range(0, sumaTotal);
        float acumulador = 0f;

        for (int i = 0; i < mensajesPrefabs.Length; i++)
        {
            acumulador += probabilidades[i];
            if (valorAleatorio <= acumulador)
            {
                return mensajesPrefabs[i];
            }
        }

        return mensajesPrefabs[0];
    }

    IEnumerator DesplazarMensaje(RectTransform mensajeRect)
    {
        Vector2 posicionFinal = new Vector2(Screen.width / 2 - margenDerecho * 2, mensajeRect.anchoredPosition.y);
        while (Vector2.Distance(mensajeRect.anchoredPosition, posicionFinal) > 0.1f)
        {
            mensajeRect.anchoredPosition = Vector2.MoveTowards(mensajeRect.anchoredPosition, posicionFinal, velocidadDesplazamiento * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator DesvanecerMensaje(CanvasGroup canvasGroup)
    {
        float tiempo = 0;
        while (tiempo < duracionDesvanecimiento)
        {
            tiempo += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(1, 0, tiempo / duracionDesvanecimiento);
            yield return null;
        }
    }
}
