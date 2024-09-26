using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private string studentSceneName;
    [SerializeField] private string teacherSceneName;

    public void LoadNextScene(string userRole)
    {
        switch (userRole.ToLower())
        {
            case "estudiante":
                SceneManager.LoadScene(studentSceneName);
                break;
            case "profesor":
                SceneManager.LoadScene(teacherSceneName);
                break;
            default:
                Debug.LogError($"Rol de usuario desconocido: {userRole}");
                break;
        }
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
