using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;

#if UNITY_ANDROID
using UnityEngine.Android;
#endif

public class YouTubePlayer : MonoBehaviour
{
    public string videoUrl = "https://www.youtube.com/watch?v=exLQLsWq1kM";
    private WebViewObject webViewObject;

    void Start()
    {
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
#endif

        StartCoroutine(InitWebView());
    }

    IEnumerator InitWebView()
    {
        // Verificar si el tipo WebViewObject existe
        Type webViewType = Type.GetType("WebViewObject");
        if (webViewType == null)
        {
            Debug.LogError("El tipo WebViewObject no se encontró. Asegúrate de que el plugin unity-webview esté correctamente importado.");
            yield break;
        }

        webViewObject = new GameObject("WebViewObject").AddComponent(webViewType) as WebViewObject;
        if (webViewObject == null)
        {
            Debug.LogError("No se pudo crear una instancia de WebViewObject.");
            yield break;
        }

        webViewObject.Init(
            cb: (msg) => {
                Debug.Log($"CallFromJS[{msg}]");
                if (msg.StartsWith("onPlayerReady"))
                {
                    webViewObject.EvaluateJS("playVideo()");
                }
            },
            err: (msg) => {
                Debug.LogError($"CallOnError[{msg}]");
            },
            started: (msg) => {
                Debug.Log($"CallOnStarted[{msg}]");
            },
            hooked: (msg) => {
                Debug.Log($"CallOnHooked[{msg}]");
            },
            ld: (msg) => {
                Debug.Log($"CallOnLoaded[{msg}]");
                webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
            }
        );

        string videoId = ExtractVideoId(videoUrl);
        string htmlPath = Path.Combine(Application.streamingAssetsPath, "YouTubePlayer.html");

        string src;
#if UNITY_EDITOR || UNITY_STANDALONE_OSX || UNITY_STANDALONE_WIN
        src = File.ReadAllText(htmlPath);
#else
        WWW www = new WWW(htmlPath);
        yield return www;
        src = www.text;
#endif
        src = src.Replace("VIDEO_ID_AQUI", videoId);

        webViewObject.LoadHTML(src, "");
        webViewObject.SetMargins(0, 0, 0, 0);
        webViewObject.SetVisibility(true);

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        // En Windows, ajustamos el tamaño del WebView
        webViewObject.SetTextZoom(100);
        // Intentamos ajustar el tamaño del WebView usando reflexión
        try
        {
            var setRectMethod = webViewType.GetMethod("SetRect");
            if (setRectMethod != null)
            {
                setRectMethod.Invoke(webViewObject, new object[] { 0, 0, Screen.width, Screen.height });
            }
            else
            {
                Debug.LogWarning("El método SetRect no está disponible. El WebView puede no ajustarse correctamente a la pantalla.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error al intentar ajustar el tamaño del WebView: {e.Message}");
        }
#endif

        yield return null;
    }

    private string ExtractVideoId(string url)
    {
        string[] patterns = new string[]
        {
            @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)",
            @"^(.*?)(^|/|v=)([a-z0-9_-]{11})(.*)?$"
        };

        foreach (string pattern in patterns)
        {
            Match match = Regex.Match(url, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
        }

        Debug.LogError("No se pudo extraer el ID del video de la URL proporcionada.");
        return string.Empty;
    }

    public void PlayVideo()
    {
        webViewObject.EvaluateJS("playVideo()");
    }

    public void PauseVideo()
    {
        webViewObject.EvaluateJS("pauseVideo()");
    }

    public void LoadVideo(string newVideoUrl)
    {
        string newVideoId = ExtractVideoId(newVideoUrl);
        if (!string.IsNullOrEmpty(newVideoId))
        {
            webViewObject.EvaluateJS($"loadVideo('{newVideoId}')");
        }
    }
}