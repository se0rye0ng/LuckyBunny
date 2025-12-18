using UnityEngine;
using UnityEngine.SceneManagement;

public class LGameController : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName = "Level2";

    // Loads the configured next scene. Call from player when appropriate.
    public void GoToNextLevel()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("LGameController: nextSceneName is empty. Cannot load next level.");
            return;
        }
        SceneManager.LoadScene(nextSceneName);
    }
}

