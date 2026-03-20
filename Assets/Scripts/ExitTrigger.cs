using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ExitTrigger : MonoBehaviour
{
    public GameManager gameManager;

    [Header("UI 설정")]
    public TextMeshProUGUI anomalyPromptText; // "이상현상 있었나요?" UI

    private bool playerInside = false;

    void Update()
    {
        if (playerInside)
        {
            // Y키 = 이상현상 있음
            if (Input.GetKeyDown(KeyCode.Y))
            {
                if (anomalyPromptText != null)
                    anomalyPromptText.gameObject.SetActive(false);
                gameManager.OnPlayerExit(true);
            }
            // N키 = 이상현상 없음
            else if (Input.GetKeyDown(KeyCode.N))
            {
                if (anomalyPromptText != null)
                    anomalyPromptText.gameObject.SetActive(false);
                gameManager.OnPlayerExit(false);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            if (anomalyPromptText != null)
            {
                anomalyPromptText.text = "이상현상이 있었나요?\nY = 있음  /  N = 없음";
                anomalyPromptText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            if (anomalyPromptText != null)
                anomalyPromptText.gameObject.SetActive(false);
        }
    }
}