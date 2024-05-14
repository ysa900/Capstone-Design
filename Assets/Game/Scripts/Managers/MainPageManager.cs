using UnityEngine;

public class MainPageManager : MonoBehaviour
{
    private LobbyManager lobbyManager;

    private void Awake()
    {
        lobbyManager = FindAnyObjectByType<LobbyManager>();
    }

    private void Start()
    {
        // 시작 시 MainPage 제외 비활성화
        lobbyManager.CharacterPage.SetActive(false);
  

        // Exit 버튼 눌렀을 때
        UnityEngine.UI.Button ExitButton = lobbyManager.ExitButtonObject.GetComponent<UnityEngine.UI.Button>();
        ExitButton.onClick.AddListener(ExitButtonClicked);
        // Character 버튼 눌렀을 때
        UnityEngine.UI.Button CharacterButton = lobbyManager.CharacterButtonObject.GetComponent<UnityEngine.UI.Button>();
        CharacterButton.onClick.AddListener(CharacterButtonClicked);
        // MainPage의 Option 버튼 눌렀을 때
        UnityEngine.UI.Button Main_OptionButton = lobbyManager.MainPageOptionButtonObject.GetComponent<UnityEngine.UI.Button>();
        Main_OptionButton.onClick.AddListener(Main_OptionButtonClicked);

    }

    // Exit 버튼 눌렀을 때
    private void ExitButtonClicked()
    {
        // 유니티 에디터에서 게임 플레이할 땐 종료 위한 #if  사용
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else // 실제 빌드 후 게임 플레이 후 종료 할 때 #else 문 사용
                Application.Quit(); // 게임 종료
        #endif
    }

    // Character 버튼 클릭 시
    private void CharacterButtonClicked()
    {
        lobbyManager.MainPage.SetActive(false);
        lobbyManager.CharacterPage.SetActive(true);
        lobbyManager.CharacterExplainGroup.SetActive(false);

        lobbyManager.isMainPageOn = false;
        lobbyManager.isCharacterPageOn = true;
        lobbyManager.gameStartButtonObject.interactable = false;
    }

    // Main Page의 Option 버튼 클릭 시
    private void Main_OptionButtonClicked()
    {
        if(!lobbyManager.isSettingPageOn) // SettingPage 열려있지 않다면
        {
            lobbyManager.isSettingPageOn = true;
            lobbyManager.SettingPage.SetActive(true);

            // 다른 버튼들 비활성화
            lobbyManager.CharacterButtonObject.interactable = false; // Character 버튼 비활성화
            lobbyManager.ExitButtonObject.interactable = false; // Exit 버튼 비활성화
        }
        else // SettingPage 열려있다면
        {
            lobbyManager.isSettingPageOn = false;
            lobbyManager.SettingPage.SetActive(false);

            // 다른 버튼들 비활성화
            lobbyManager.CharacterButtonObject.interactable = true; // Character 버튼 비활성화
            lobbyManager.ExitButtonObject.interactable = true; // Exit 버튼 비활성화
        }

        



    }


}
