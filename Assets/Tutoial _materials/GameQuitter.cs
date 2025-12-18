using UnityEngine;

public class GameQuitter : MonoBehaviour
{
    void Update()
    {
        // 매 프레임마다 ESC 키가 눌렸는지 확인합니다.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            QuitGame();
        }
    }

    // 버튼의 On Click() 이벤트에서도 이 함수를 그대로 사용하면 됩니다.
    public void QuitGame()
    {
        Debug.Log("게임 종료를 시도합니다.");

        // 1. 실제 빌드된 게임을 종료
        Application.Quit();

        // 2. 유니티 에디터에서 테스트 중일 때 재생 모드를 종료
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}