// StartMenuUI.cs - Canvas에 붙이기
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class StartMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private EngineAmbience engineAmbience;
    [SerializeField] private string nextSceneName;
    [SerializeField] private float fadeoutTime;
    private bool isStarting = false; 
    void Start()
    {
        startPanel.SetActive(true);
    }

    public void OnStartButton()
    {
        if (isStarting) return; // 이미 시작 중이면 무시

        StartCoroutine(StartGameRoutine());
    }

    private IEnumerator StartGameRoutine()
    {
        isStarting = true;

        // 1. UI 먼저 숨기기
        startPanel.SetActive(false);

        // 2. EngineAmbience의 모든 연출이 끝날 때까지 대기
        // yield return을 사용하여 StopAllSequence가 끝날 때까지 멈춤
        yield return StartCoroutine(engineAmbience.StopAllSequence());

        yield return StartCoroutine(SceneFader.Instance.FadeOut(fadeoutTime));
        // 3. 연출이 모두 끝난 뒤 씬 전환
        Debug.Log("모든 연출 종료, 씬 전환 시작!");
        GameSceneManager.LoadScene(nextSceneName);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}

