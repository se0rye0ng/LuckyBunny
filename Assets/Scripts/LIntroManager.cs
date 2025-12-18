using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video; // 비디오 기능을 쓰기 위해 필수!

public class IntroManager : MonoBehaviour
{
    public VideoPlayer videoPlayer; // 아까 만든 비디오 플레이어
    public string nextSceneName = "Level01_01"; // 넘어갈 씬 이름

    void Start()
    {
        // 비디오 플레이어가 연결되어 있다면
        if (videoPlayer != null)
        {
            // "영상이 끝(loopPointReached)나면 OnVideoEnd 함수를 실행해라"라고 예약
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    void Update()
    {
        // 플레이어가 지루해서 스페이스바나 ESC를 누르면 바로 스킵
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape))
        {
            LoadNextScene();
        }
    }

    // 영상이 끝났을 때 자동으로 호출됨
    void OnVideoEnd(VideoPlayer vp)
    {
        LoadNextScene();
    }

    // 다음 씬으로 이동하는 함수
    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}