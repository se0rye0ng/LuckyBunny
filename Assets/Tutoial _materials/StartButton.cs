using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void LoadTutorial()
    {
        // 씬 이름이 정확히 "Tutorial"인지 확인하세요!
        SceneManager.LoadScene("Tutorial"); 
    }
}