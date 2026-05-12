using UnityEngine;


public class FadeSignalReceiver : MonoBehaviour
{
    public float fadeDuration = 1f;

    // Signal에서 호출할 함수들
    public void OnFadeOut()
    {
        StartCoroutine(SceneFader.Instance.FadeOut(fadeDuration));
    }

    public void OnFadeIn()
    {
        StartCoroutine(SceneFader.Instance.FadeIn(fadeDuration));
    }
}