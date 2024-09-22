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

    // Velocidad de desplazamiento
    public float velocidadDesplazamiento = 200f;
    // Duración del desvanecimiento
    public float duracionDesvanecimiento = 2.0f;

    private float tiempoTranscurrido = 0.0f;

    void Start()
    {
        StartCoroutine(GenerarMensajesAleatoriamente());
    }

    IEnumerator GenerarMensajesAleatoriamente()
    {
        while (tiempoTranscurrido < duracionTotal)
        {
            GameObject mensajeAleatorio = mensajesPrefabs[Random.Range(0, mensajesPrefabs.Length)];
            GameObject mensajeInstanciado = Instantiate(mensajeAleatorio, canvasTransform);

            // Inicialmente, colocamos el mensaje fuera de la pantalla (a la derecha)
            RectTransform mensajeRect = mensajeInstanciado.GetComponent<RectTransform>();
            mensajeRect.anchoredPosition = new Vector2(Screen.width / 2 + margenDerecho, Random.Range(-margenVertical, margenVertical));

            // Comenzar la animación de desplazamiento hacia la izquierda
            StartCoroutine(DesplazarMensaje(mensajeRect));

            // Esperar antes de destruir el mensaje (tiempo de vida)
            yield return new WaitForSeconds(duracionMensaje - duracionDesvanecimiento);

            // Comenzar la animación de desvanecimiento antes de destruir el mensaje
            CanvasGroup canvasGroup = mensajeInstanciado.GetComponent<CanvasGroup>();
            StartCoroutine(DesvanecerMensaje(canvasGroup));

            // Destruir el mensaje después de que el desvanecimiento haya terminado
            Destroy(mensajeInstanciado, duracionDesvanecimiento);

            yield return new WaitForSeconds(intervaloMensajes);
            tiempoTranscurrido += intervaloMensajes;
        }
    }

    // Corutina para desplazar el mensaje de derecha a izquierda
    IEnumerator DesplazarMensaje(RectTransform mensajeRect)
    {
        Vector2 posicionFinal = new Vector2(Screen.width / 2 - margenDerecho, mensajeRect.anchoredPosition.y);
        while (Vector2.Distance(mensajeRect.anchoredPosition, posicionFinal) > 0.1f)
        {
            mensajeRect.anchoredPosition = Vector2.MoveTowards(mensajeRect.anchoredPosition, posicionFinal, velocidadDesplazamiento * Time.deltaTime);
            yield return null;
        }
    }

    // Corutina para desvanecer el mensaje gradualmente
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
