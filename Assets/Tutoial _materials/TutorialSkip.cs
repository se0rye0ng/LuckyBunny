using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환 필수

public class TutorialSkip : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private string gameSceneName = "GameScene"; // 이동할 게임 장면 이름

    // 버튼을 클릭했을 때 실행할 함수
    public void SkipTutorial()
    {
        Debug.Log("튜토리얼 스킵! 게임 장면으로 이동합니다.");
        
        // 이동할 씬 이름이 설정되어 있는지 확인 후 로드
        if (!string.IsNullOrEmpty(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError("이동할 게임 씬 이름이 입력되지 않았습니다!");
        }
    }
}