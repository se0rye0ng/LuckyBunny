using UnityEngine;
using UnityEngine.UI; // 이게 있어야 텍스트를 제어합니다
using UnityEngine.SceneManagement;

public class GameTimer : MonoBehaviour
{
    [Header("설정")]
    public float timeLimit = 60f; // 제한시간 (60초)
    public Text timerText; // 여기에 아까 만든 TimerText를 넣을 겁니다

    private float currentTime;
    private bool isGameActive = true;

    void Start()
    {
        currentTime = timeLimit;
    }

    void Update()
    {
        if (!isGameActive) return;

        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            // 화면에 시간 표시 (00:00 형식)
            if (timerText != null)
            {
                // 소수점 버리고 정수로 표시
                timerText.text = Mathf.Ceil(currentTime).ToString(); 
            }
        }
        else
        {
            Debug.Log("게임 오버!");
            currentTime = 0;
            isGameActive = false;
            
            // 시간이 다 되면 현재 씬 재시작 (원하는 대로 바꾸세요)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    // 외부에서 타이머 멈출 때 사용 (클로버 먹었을 때 등)
    public void StopTimer()
    {
        isGameActive = false;
    }
}