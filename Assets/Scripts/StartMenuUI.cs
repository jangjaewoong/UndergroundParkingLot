// StartMenuUI.cs - Canvas에 붙이기
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private EngineAmbience engineAmbience;

    void Start()
    {
        startPanel.SetActive(true);
    }

    public void OnStartButton()
    {
        engineAmbience.StopAll();       // 진동 + 소리 정지
        startPanel.SetActive(false);    // UI 숨기기
        // SceneManager.LoadScene("GameScene"); // 씬 전환할 경우
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}