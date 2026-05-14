using UnityEngine;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
public class DialoagueTest : MonoBehaviour
{
    [SerializeField] private GameObject dialogueCanvas;
    [SerializeField] private TMP_Text dialogueText;
    
    [SerializeField] LocalizeStringEvent dialogueEvent;
    [SerializeField] private string tableName;

    [SerializeField] private int dialogueIndex = 1;
    private int currentIndex = -1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(dialogueCanvas.activeSelf);
        if (!dialogueCanvas.activeSelf) // 게임 오브젝트가 비활성화 상태인 경우
        {
            dialogueCanvas.SetActive(true);
        }

        UpdateDialogue();

    }

    // Update is called once per frame
    void Update()
    {
        if (currentIndex != dialogueIndex)
        {
            UpdateDialogue();
        }
    }

    public void UpdateDialogue()
    {
        currentIndex = dialogueIndex;
        // 테이블과 키 동시 설정.
        dialogueEvent.StringReference.SetReference(tableName, $"Dial_{currentIndex}");

        // Referece 변경 후 강제 갱신
        dialogueEvent.RefreshString();
    }

}
