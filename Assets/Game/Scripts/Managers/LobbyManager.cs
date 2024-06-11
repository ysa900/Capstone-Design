using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    // 버튼 모음
    // gameStart 버튼
    public UnityEngine.UI.Button gameStartButtonObject;

    // Character 버튼
    public UnityEngine.UI.Button CharacterButtonObject;

    // Character Page Back 버튼
    public UnityEngine.UI.Button CharacterPageBackButtonObject;

    // Setting Page(옵션 창)의 Back 버튼
    public UnityEngine.UI.Button SettingPageBackButtonObject;

    // Exit 버튼
    public UnityEngine.UI.Button ExitButtonObject;

    // Main Page Option 버튼
    public UnityEngine.UI.Button MainPageOptionButtonObject;

    // Character Page Option 버튼
    public UnityEngine.UI.Button CharacterPageOptionButtonObject;

    // 캐릭터 선택창 관련
    // 캐릭터 선택 및 스토리 패널
    public GameObject CharacterExplainGroup;

    // 캐릭터(3) 선택
    public UnityEngine.UI.Button SelectAssassinButtonObject; // Assasin
    public UnityEngine.UI.Button SelectMageButtonObject; // Mage
    public UnityEngine.UI.Button SelectWarriorButtonObject; // Warrior

    // 페이지 모음
    // Main Page 오브젝트
    public GameObject MainPage;

    // Character Page 오브젝트
    public GameObject CharacterPage;

    // Setting Page 오브젝트
    public GameObject SettingPage;

    // bool 변수 모음
    public bool isCharacterSelect = false; // 캐릭터 선택창이 켜졌는지를 확인할 때 필요한 bool 변수
    public bool isCharacterPageOn = false;
    public bool isMainPageOn = true; // MainPage 켜져있는 지 확인하는 bool 변수
    public bool isSettingPageOn = false;
    public bool isSettingSave = false; // Save 버튼 눌렀는지 확인하는 bool 변수
    //public bool isGameStartEnabled = false;

    private void Start()
    {
        // 시작 시 비활성화
        CharacterPage.SetActive(false);

        SettingPage.SetActive(false);

    }
}