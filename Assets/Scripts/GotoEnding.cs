using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GotoEnding : MonoBehaviour
{
    [Header("이동 설정")]
    public string nextSceneName = "EndingScene";
    public float waitTime = 5.0f;

    void Start()
    {
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        yield return new WaitForSeconds(waitTime);

        Debug.Log("엔딩 씬으로 이동합니다: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}