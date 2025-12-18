using UnityEngine;
using UnityEngine.SceneManagement;

public class GotoLv1 : MonoBehaviour
{
    [Header("Scene Settings")]
    public string nextSceneName = "level01_Intro";

    // [추가된 부분] 플레이어가 닿으면 자동으로 실행됨
    private void OnTriggerEnter2D(Collider2D other)
    {
        // 닿은 게 플레이어인지 확인 (Tag가 Player인지 꼭 확인하세요!)
        if (other.CompareTag("Player"))
        {
            GoToNextLevel();
        }
    }

    public void GoToNextLevel()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("Scene Name is Empty!");
            return;
        }
        SceneManager.LoadScene(nextSceneName);
    }
}