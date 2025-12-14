using UnityEngine;

[System.Obsolete("OPlayerController is deprecated. Use LPlayerController instead.")]
public class OPlayerController : MonoBehaviour
{
    void Awake()
    {
        Debug.LogWarning("OPlayerController is deprecated. Use LPlayerController instead.");
    }
}
