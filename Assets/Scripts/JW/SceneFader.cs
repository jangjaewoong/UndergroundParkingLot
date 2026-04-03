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

    private void OnEnable()
    {
        // 씬 로드가 완료되었을 때 실행될 함수 등록
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 오브젝트 파괴 시 등록 해제 (메모리 누수 방지)
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 새로운 씬이 로드되면 1초 동안 화면을 밝힙니다.
        StartCoroutine(FadeIn(fadeinTime));
    }
}