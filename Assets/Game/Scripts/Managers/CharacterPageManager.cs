using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterPageManager : MonoBehaviour
{
    private LobbyManager lobbyManager;


    private void Awake()
    {
        lobbyManager = FindAnyObjectByType<LobbyManager>();
    }

    void Start()
    {

        // 현재 클래스 구현 1개 - Mage
        // 캐릭터(Mage) 선택 시
        UnityEngine.UI.Button SelectMageButton = lobbyManager.SelectMageButtonObject.GetComponent<UnityEngine.UI.Button>();
        SelectMageButton.onClick.AddListener(SelectMageButtonClicked);
        // Character Page 뒤로가기 버튼 눌렀을 때
        UnityEngine.UI.Button CharacterPageBackButton = lobbyManager.CharacterPageBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        CharacterPageBackButton.onClick.AddListener(CharacterPage_BackButtonClicked);
        // Character Page의 Option 버튼 누를 때
        UnityEngine.UI.Button CharacterPage_OptionButton = lobbyManager.CharacterPageOptionButtonObject.GetComponent<UnityEngine.UI.Button>();
        CharacterPage_OptionButton.onClick.AddListener(CharacterPage_OptionButtonClicked);
        // GameStart 버튼 눌렀을 때
        UnityEngine.UI.Button gameStartButton = lobbyManager.gameStartButtonObject.GetComponent<UnityEngine.UI.Button>();
        gameStartButton.onClick.AddListener(GameStartButtonClicked);
    }

    // Mage 선택 시
    private void SelectMageButtonClicked()
    {
        lobbyManager.CharacterExplainGroup.SetActive(true);

        //lobbyManager.isGameStartEnabled = true;
        lobbyManager.isCharacterSelect = true;
        lobbyManager.SelectAssassinButtonObject.enabled = false;
        lobbyManager.SelectWarriorButtonObject.enabled = false;

        lobbyManager.gameStartButtonObject.interactable = true;
    }

    // Character Page 뒤로가기 버튼 클릭 시
    private void CharacterPage_BackButtonClicked()
    {
        lobbyManager.CharacterPageOptionButtonObject.enabled = true; // 옵션 버튼 재활성화

        // 캐릭터 선택도 안하고 옵션창도 떠있지 않은 상황
        if (!lobbyManager.isCharacterSelect && !lobbyManager.isSettingPageOn)
        {
            lobbyManager.CharacterPage.SetActive(false);
            lobbyManager.MainPage.SetActive(true);

            lobbyManager.isMainPageOn = true;
            lobbyManager.isSettingPageOn = false;
        }

        // 캐릭터 선택되어 있지 않고 옵션창만 떠있는 상황
        else if (!lobbyManager.isCharacterSelect && lobbyManager.isSettingPageOn)
        {
            // 어차피 뒤로가기 버튼 비활성화 되어있음
        }
        // 캐릭터 선택되어 있고 옵션창은 안떠있는 상황
        else if (lobbyManager.isCharacterSelect && !lobbyManager.isSettingPageOn)
        {
            lobbyManager.CharacterExplainGroup.SetActive(false);
            // 캐릭터 선택 버튼들 활성화
            lobbyManager.SelectAssassinButtonObject.enabled = true;
            lobbyManager.SelectWarriorButtonObject.enabled = true;
            lobbyManager.SelectMageButtonObject.enabled = true;

            lobbyManager.isCharacterSelect = false;
            lobbyManager.gameStartButtonObject.interactable = false;
        }
        // 캐릭터 선택되어 있고 옵션창도 떠있는 상황
        else
        {
            // 어차피 뒤로가기 버튼 비활성화 되어있음
        }
    }

    private void CharacterPage_OptionButtonClicked()
    {
        // 옵션창 안켜진 상황에서 누르기
        if (!lobbyManager.SettingPage.activeSelf)
        {
            lobbyManager.SettingPage.SetActive(true);
            lobbyManager.CharacterPageBackButtonObject.enabled = false; // CharacterPage의 뒤로가기 버튼 무효화

            // 캐릭터 선택 버튼들 무효화
            lobbyManager.SelectAssassinButtonObject.enabled = false;
            lobbyManager.SelectWarriorButtonObject.enabled = false;
            lobbyManager.SelectMageButtonObject.enabled = false;

            // Character Page에서
            // 캐릭터 선택하지 않은 상황에서 Option 버튼 누르기
            if (!lobbyManager.isCharacterSelect && lobbyManager.isCharacterPageOn)
            {
                lobbyManager.isSettingPageOn = true;
                lobbyManager.gameStartButtonObject.interactable = false;
                //lobbyManager.CharacterPageOptionButtonObject.enabled = false;
            }
            // 캐릭터 선택한 상황에서 Option 버튼 누르기
            else if (lobbyManager.isCharacterSelect && lobbyManager.isCharacterPageOn)
            {
                lobbyManager.isSettingPageOn = true;
                //lobbyManager.gameStartButtonObject.interactable = false;
            }
        }
        // 옵션창에서 켜진 상황에서 또 누르기
        else
        {
            lobbyManager.isSettingPageOn = false;
            lobbyManager.SettingPage.SetActive(false);
            lobbyManager.CharacterPageBackButtonObject.enabled = true;
            // 캐릭터 선택하지 않은 상황
            if (!lobbyManager.isCharacterSelect && lobbyManager.isCharacterPageOn)
            {
                lobbyManager.gameStartButtonObject.interactable = false;

                // 캐릭터 선택 버튼들 활성화
                lobbyManager.SelectAssassinButtonObject.enabled = true;
                lobbyManager.SelectWarriorButtonObject.enabled = true;
                lobbyManager.SelectMageButtonObject.enabled = true;

            }
            // 캐릭터 선택한 상황
            else if (lobbyManager.CharacterExplainGroup.activeSelf || lobbyManager.isCharacterSelect && lobbyManager.isCharacterPageOn)
            {
                lobbyManager.CharacterPageOptionButtonObject.enabled = true;

                lobbyManager.SelectAssassinButtonObject.enabled = false;
                lobbyManager.SelectWarriorButtonObject.enabled = false;
                lobbyManager.SelectMageButtonObject.enabled = false;

                lobbyManager.gameStartButtonObject.interactable = true;
            }
        }


    }

    // GameStart 버튼 클릭 시
    private void GameStartButtonClicked()
    {
        SceneManager.LoadScene("Splash1"); // 1회차: Lobby -> Splash1
    }
}