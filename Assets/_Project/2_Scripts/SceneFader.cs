using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class SceneFader : MonoBehaviour
{
    // 싱글톤 인스턴스
    public static SceneFader Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeinTime;

    private void Awake()
    {
        // 싱글톤 설정 및 씬 전환 시 파괴 방지
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 화면을 어둡게 (Fade Out)
    public IEnumerator FadeOut(float duration)
    {
        yield return StartCoroutine(Fade(0, 1, duration));
        fadeCanvasGroup.blocksRaycasts = true; // 클릭 방지
    }

    // 화면을 밝게 (Fade In)
    public IEnumerator FadeIn(float duration)
    {
        fadeCanvasGroup.blocksRaycasts = true;
        yield return StartCoroutine(Fade(1, 0, duration));
        fadeCanvasGroup.blocksRaycasts = false; // 클릭 방해 해제
    }

    private IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            yield return null;
        }
        fadeCanvasGroup.alpha = endAlpha;
    }

    // ⭐ 추가: 즉시 검은색 강제
    public void SetBlackInstant()
    {
        fadeCanvasGroup.alpha = 1f;
        fadeCanvasGroup.blocksRaycasts = true;
    }

    // ⭐ 추가: 현재 alpha 확인용 (디버그)
    public float CurrentAlpha => fadeCanvasGroup.alpha;

}


