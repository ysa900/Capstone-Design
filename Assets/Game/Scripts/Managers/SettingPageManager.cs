using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPageManager : MonoBehaviour
{
    private LobbyManager lobbyManager;

    private void Awake()
    {
        lobbyManager = FindAnyObjectByType<LobbyManager>();
    }

    void Start()
    {
        // SettingPage 뒤로가기 버튼 눌렀을 때
        UnityEngine.UI.Button SettingPageBackButton = lobbyManager.SettingPageBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        SettingPageBackButton.onClick.AddListener(SettingPageBackButtonClicked);
    }

    // OptionPage 뒤로가기 버튼 클릭 시
    // 1. MainPage에서
    //  1-1. 그냥 옵션창 끄기 끝.
    // 2. CharacterPage에서(전제상황: 옵션창 켜진 거)
    //  2-1. 캐릭터 선택되지 않은 상황에서 옵션창 끄기
    //  2-2. 캐릭터 선택되어 있는 상황에서 옵션창 끄기
    private void SettingPageBackButtonClicked()
    {
        lobbyManager.isSettingPageOn = false;
        lobbyManager.SettingPage.SetActive(false);

        // MainPage인 상황
        if (lobbyManager.MainPage.activeSelf) // lobbyManager.isMainPageOn true 확인과 같은 문구
        {
            // 버튼 재활성화시키기
            lobbyManager.CharacterButtonObject.enabled = true;
            lobbyManager.ExitButtonObject.enabled = true;
            lobbyManager.MainPageOptionButtonObject.enabled = true;
        }
        // CharacterPage인 상황
        else if (lobbyManager.isCharacterPageOn && !lobbyManager.isMainPageOn)
        {
            //캐릭터 선택되지 않은 상황에서 옵션창 끄기
            if (!lobbyManager.isCharacterSelect)
            {
                // 버튼 재활성화시키기
                lobbyManager.CharacterPageBackButtonObject.enabled = true;
                lobbyManager.SelectAssassinButtonObject.enabled = true;
                lobbyManager.SelectWarriorButtonObject.enabled = true;
                lobbyManager.SelectMageButtonObject.enabled = true;
                lobbyManager.CharacterPageOptionButtonObject.enabled = true;

            }
            // 캐릭터 선택되어 있는 상황에서 옵션창 끄기
            else if (lobbyManager.isCharacterSelect)
            {
                // 캐릭선택 후 설명화면만 남기고 캐릭선택 버튼 비활성화하고 GameStart 버튼은 활성화
                lobbyManager.SelectAssassinButtonObject.enabled = false;
                lobbyManager.SelectWarriorButtonObject.enabled = false;
                lobbyManager.SelectMageButtonObject.enabled = false;

                lobbyManager.CharacterPageBackButtonObject.enabled = true;
                lobbyManager.CharacterPageOptionButtonObject.enabled = true;
                lobbyManager.gameStartButtonObject.interactable = true;
            }
        }
    }
}