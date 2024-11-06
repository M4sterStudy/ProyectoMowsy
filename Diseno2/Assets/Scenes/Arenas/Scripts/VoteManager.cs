using UnityEngine;
using UnityEngine.UI;

public class VoteManager : MonoBehaviour
{
    public Button sportsButton, videoGamesButton, cinemaButton, musicButton, literatureButton;

    private void Start()
    {
        // Asegurar que los votos comiencen en 0
        if (!PlayerPrefs.HasKey("SportsVotes")) ResetVotes();

        // Asignar los listeners a cada botón
        sportsButton.onClick.AddListener(() => RegisterVote("SportsVotes"));
        videoGamesButton.onClick.AddListener(() => RegisterVote("VideoGamesVotes"));
        cinemaButton.onClick.AddListener(() => RegisterVote("CinemaVotes"));
        musicButton.onClick.AddListener(() => RegisterVote("MusicVotes"));
        literatureButton.onClick.AddListener(() => RegisterVote("LiteratureVotes"));
    }

    private void RegisterVote(string category)
    {
        int currentVotes = PlayerPrefs.GetInt(category);
        PlayerPrefs.SetInt(category, currentVotes + 1);
        PlayerPrefs.Save();
        Debug.Log($"{category} tiene ahora {currentVotes + 1} votos.");
    }

    public void ResetVotes()
    {
        PlayerPrefs.SetInt("SportsVotes", 0);
        PlayerPrefs.SetInt("VideoGamesVotes", 0);
        PlayerPrefs.SetInt("CinemaVotes", 0);
        PlayerPrefs.SetInt("MusicVotes", 0);
        PlayerPrefs.SetInt("LiteratureVotes", 0);
        PlayerPrefs.Save();
    }
}
