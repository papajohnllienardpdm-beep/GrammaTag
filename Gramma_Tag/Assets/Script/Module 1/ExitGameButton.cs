using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGameButton : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu"; // pangalan ng main menu scene mo

    public void ExitGame()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }
}