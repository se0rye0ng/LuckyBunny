using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GotoLv2 : MonoBehaviour
{
    [Header("이동 설정")]
    public string nextSceneName = "Level2";
    public float waitTime = 5.0f;

    void Start()
    {
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        yield return new WaitForSeconds(waitTime);

        Debug.Log("Level2로 이동합니다: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}