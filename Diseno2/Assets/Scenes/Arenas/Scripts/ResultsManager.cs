using UnityEngine;
using TMPro; // Para TextMeshPro
using UnityEngine.UI;

public class ResultsManager : MonoBehaviour
{
    public TMP_Text sportsVotesText, videoGamesVotesText, cinemaVotesText, musicVotesText;

    private void Start()
    {
        // Obtener los votos de PlayerPrefs y mostrarlos
        int sportsVotes = PlayerPrefs.GetInt("SportsVotes");
        int videoGamesVotes = PlayerPrefs.GetInt("VideoGamesVotes");
        int cinemaVotes = PlayerPrefs.GetInt("CinemaVotes");
        int musicVotes = PlayerPrefs.GetInt("MusicVotes");

        sportsVotesText.text = $"Deportes: {sportsVotes}";
        videoGamesVotesText.text = $"Videojuegos: {videoGamesVotes}";
        cinemaVotesText.text = $"Cine: {cinemaVotes}";
        musicVotesText.text = $"Música: {musicVotes}";
    }
}
