using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEngine.Localization.Settings;
public class LocaleSelector : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int ID = PlayerPrefs.GetInt("LocaleKey", 0);
        // 기기 로컬 저장소의 데이터에 접근해 LocaleKey라는 키가 존재하면 해당 값을 가져오고 없으면 0을 반환.
        // 게임 시작 시 플레이어 로컬 언어로 설정, 없으면 기본 언어(EN)로 설정함.
        ChangeLocale(ID);
    }

    private bool isActive = false;

    public void ChangeLocale(int localeID)
    {
        if (isActive == true)
        {
            return;
        }
        // 언어 변경 키를 연속적으로 눌렀을 때 콜백이 쌓이는걸 방지하기 위해 코루틴 사용
        StartCoroutine(SetLocale(localeID));
    }

    // 코루틴 만들기 위한 형식
    IEnumerator SetLocale(int _localeID)
    {
        isActive = true;
        // 유니티 로컬라이징 설정들을 읽어올 때까지 대기
        yield return LocalizationSettings.InitializationOperation;
        // 현재 선택된 로컬 언어를 _localeID 번째 언어로 변경.
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[_localeID];
        isActive = false;
    }


    
}
