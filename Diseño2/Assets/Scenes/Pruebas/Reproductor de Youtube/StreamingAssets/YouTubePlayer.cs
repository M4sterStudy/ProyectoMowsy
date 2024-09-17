using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;

public class YouTubePlayer : MonoBehaviour
{
    public string videoUrl = "https://www.youtube.com/watch?v=exLQLsWq1kM";
    //private WebViewObject webViewObject;

    void Start()
    {
        //    string videoId = ExtractVideoId(videoUrl);

        //    //// Crear el WebView
        //    //webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        //    //webViewObject.Init(cb: (msg) => {
        //    //    Debug.Log($"WebView message: {msg}");
        //    //});

        //    // Cargar el HTML
        //    string htmlPath = Path.Combine(Application.streamingAssetsPath, "YouTubePlayer.html");
        //    string html = File.ReadAllText(htmlPath);
        //    html = html.Replace("VIDEO_ID_AQUI", videoId);

        //    // Mostrar el WebView con el HTML
        //    webViewObject.LoadHTML(html);
        //    webViewObject.SetMargins(0, 0, 0, 0);
        //    webViewObject.SetVisibility(true);

        //    // Permitir la comunicación entre JavaScript y Unity
        //    webViewObject.AddCustomHeader("X-Unity-SHM", "true");
        //}

        //private string ExtractVideoId(string url)
        //{
        //    // Patrones para diferentes formatos de URL de YouTube
        //    string[] patterns = new string[]
        //    {
        //        @"youtu(?:\.be|be\.com)/(?:.*v(?:/|=)|(?:.*/)?)([a-zA-Z0-9-_]+)",
        //        @"^(.*?)(^|/|v=)([a-z0-9_-]{11})(.*)?$"
        //    };

        //    foreach (string pattern in patterns)
        //    {
        //        Match match = Regex.Match(url, pattern, RegexOptions.IgnoreCase);
        //        if (match.Success)
        //        {
        //            return match.Groups[1].Value;
        //        }
        //    }

        //    Debug.LogError("No se pudo extraer el ID del video de la URL proporcionada.");
        //    return string.Empty;
        //}

        //public void PlayVideo()
        //{
        //    webViewObject.EvaluateJS("playVideo()");
        //}

        //public void PauseVideo()
        //{
        //    webViewObject.EvaluateJS("pauseVideo()");
        //}

        //public void LoadVideo(string newVideoUrl)
        //{
        //    string newVideoId = ExtractVideoId(newVideoUrl);
        //    if (!string.IsNullOrEmpty(newVideoId))
        //    {
        //        webViewObject.EvaluateJS($"loadVideo('{newVideoId}')");
        //    }
    }
}