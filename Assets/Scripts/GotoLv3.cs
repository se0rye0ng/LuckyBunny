using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GotoLv3 : MonoBehaviour
{
    [Header("이동 설정")]
    public string nextSceneName = "Level3";
    public float waitTime = 6.0f;

    void Start()
    {
        StartCoroutine(MoveRoutine());
    }

    IEnumerator MoveRoutine()
    {
        yield return new WaitForSeconds(waitTime);

        Debug.Log("Level3로 이동합니다: " + nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }
}