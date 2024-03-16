using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    // 버튼 모음
    // gameStart 버튼
    public UnityEngine.UI.Button gameStartButtonObject;

    // Character 버튼
    public UnityEngine.UI.Button CharacterButtonObject;

    // Item 버튼
    public UnityEngine.UI.Button ItemButtonObject;

    // Character Page Back 버튼
    public UnityEngine.UI.Button CharacterPageBackButtonObject;

    // Item Page Back 버튼
    public UnityEngine.UI.Button ItemPageBackButtonObject;

    // Setting Page(옵션 창)의 Back 버튼
    public UnityEngine.UI.Button SettingPageBackButtonObject;

    // Exit 버튼
    public UnityEngine.UI.Button ExitButtonObject;

    // Main Page Option 버튼
    public UnityEngine.UI.Button MainPageOptionButtonObject;

    // Character Page Option 버튼
    public UnityEngine.UI.Button CharacterPageOptionButtonObject;

    // SettingPage의 Save 버튼
    public UnityEngine.UI.Button SaveButtonObject;

    // 캐릭터 선택창 관련
    // 캐릭터 선택 및 스토리 패널
    public GameObject CharacterExplainGroup;

    // 캐릭터(3) 선택
    public UnityEngine.UI.Button SelectAssassinButtonObject; // Assasin
    public UnityEngine.UI.Button SelectMageButtonObject; // Mage
    public UnityEngine.UI.Button SelectWarriorButtonObject; // Warrior


    public AudioSource MainPageAudio;
    public AudioSource CharacterPageAudio;

    // 페이지 모음
    // Main Page 오브젝트
    public GameObject MainPage;

    // Character Page 오브젝트
    public GameObject CharacterPage;

    // Item Page 오브젝트
    public GameObject ItemPage;

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

        MainPageAudio = MainPage.GetComponentInChildren<AudioSource>();
        CharacterPageAudio = CharacterPage.GetComponentInChildren<AudioSource>();
        // 시작 시 비활성화
        CharacterPage.SetActive(false);
        ItemPage.SetActive(false);

        // 미구현:
        UnityEngine.UI.Button SelectAssassinButton = SelectAssassinButtonObject.GetComponent<UnityEngine.UI.Button>();
        UnityEngine.UI.Button SelectWarriorButton = SelectWarriorButtonObject.GetComponent<UnityEngine.UI.Button>();

        // 미구현: Item Page 뒤로가기 버튼 눌렀을 때
        UnityEngine.UI.Button ItemPageBackButton = ItemPageBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        ItemPageBackButton.onClick.AddListener(ItemPageBackButtonClicked);
    }

    // Item Page 미구현이라 해당 버튼 함수는 여기에 둠
    // Item Page 뒤로가기 버튼 클릭 시
    private void ItemPageBackButtonClicked()
    {
        ItemPage.SetActive(false);
        MainPage.SetActive(true);
    }
}
