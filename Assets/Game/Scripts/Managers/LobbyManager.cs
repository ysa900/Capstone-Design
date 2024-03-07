using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private LobbyAudioManager lobbyAudioManager;

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

    // Setting Page Back 버튼
    public UnityEngine.UI.Button SettingPageBackButtonObject;

    // Exit 버튼
    public UnityEngine.UI.Button ExitButtonObject;

    // Main Page Option 버튼
    public UnityEngine.UI.Button MainPageOptionButtonObject;

    // Character Page Option 버튼
    public UnityEngine.UI.Button CharacterPageOptionButtonObject;

    // Character Page 오브젝트
    public GameObject CharacterPage;

    // Item Page 오브젝트
    public GameObject ItemPage;

    // Setting Page 오브젝트
    public GameObject SettingPage;

    // 캐릭터 선택 및 스토리 패널
    public GameObject CharacterExplainGroup;

    // 캐릭터(3) 선택
    public GameObject SelectAssassin; // Assasin
    public GameObject SelectMage; // Mage
    public GameObject SelectWarrior; // Warrior

    // 캐릭터 선택창이 켜졌는지를 확인할 때 필요한 bool 변수
    bool isCharacterSelect = false;

    private void Awake()
    {
        lobbyAudioManager = FindAnyObjectByType<LobbyAudioManager>();
    }

    private void Start()
    {
        //lobbyAudioManager.PlayBGM(true);
        // 시작 시 비활성화
        CharacterPage.SetActive(false);
        ItemPage.SetActive(false);

        // Restart 버튼 눌렀을 때
        UnityEngine.UI.Button gameStartButton = gameStartButtonObject.GetComponent<UnityEngine.UI.Button>();
        gameStartButton.onClick.AddListener(GameStartButtonClicked);

        // Exit 버튼 눌렀을 때
        UnityEngine.UI.Button ExitButton = ExitButtonObject.GetComponent<UnityEngine.UI.Button>();
        ExitButton.onClick.AddListener(ExitButtonClicked);

        // Character 버튼 눌렀을 때
        UnityEngine.UI.Button CharacterButton = CharacterButtonObject.GetComponent<UnityEngine.UI.Button>();
        CharacterButton.onClick.AddListener(CharacterButtonClicked);

        // Item 버튼 눌렀을 때
        UnityEngine.UI.Button AbilityButton = ItemButtonObject.GetComponent<UnityEngine.UI.Button>();
        AbilityButton.onClick.AddListener(AbilityButtonClicked);

        // Character Page 뒤로가기 버튼 눌렀을 때
        UnityEngine.UI.Button CharacterPageBackButton = CharacterPageBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        CharacterPageBackButton.onClick.AddListener(CBackButtonClicked);

        // Item Page 뒤로가기 버튼 눌렀을 때
        UnityEngine.UI.Button ItemPageBackButton = ItemPageBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        ItemPageBackButton.onClick.AddListener(ItemPageBackButtonClicked);

        // Option 뒤로가기 버튼 눌렀을 때
        UnityEngine.UI.Button SettingPageBackButton = SettingPageBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        SettingPageBackButton.onClick.AddListener(SettingPageBackButtonClicked);

        // MainPage의 Option 버튼 눌렀을 때
        UnityEngine.UI.Button Main_OptionButton = MainPageOptionButtonObject.GetComponent<UnityEngine.UI.Button>();
        Main_OptionButton.onClick.AddListener(Main_OptionButtonClicked);

        UnityEngine.UI.Button CharacterPage_OptionButton = CharacterPageOptionButtonObject.GetComponent<UnityEngine.UI.Button>();
        CharacterPage_OptionButton.onClick.AddListener(CharacterPage_OptionButtonClicked);

        // 현재 구현 1개 - Mage
        // Mage 선택 시
        UnityEngine.UI.Button SelectMageButton = SelectMage.GetComponent<UnityEngine.UI.Button>();
        SelectMageButton.onClick.AddListener(SelectMageButtonClicked);
    }

    // Exit 버튼 눌렀을 때
    private void ExitButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select);

        // 유니티 에디터에서 게임 플레이 종료 위한 #if 키워드 사용
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit(); // 게임 종
        #endif
    }
    
    // Mage 선택 시
    private void SelectMageButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select);
        CharacterExplainGroup.SetActive(true);

        isCharacterSelect = true;

        // 추가사항 필요 - GameStart 버튼 활성화
        gameStartButtonObject.interactable = true;
    }

    // GameStart 버튼 클릭 시
    private void GameStartButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select);
        SceneManager.LoadScene("Game"); // Game 씬 불러오기
    }

    // Character 버튼 클릭 시
    private void CharacterButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select);
        CharacterPage.SetActive(true);
        CharacterExplainGroup.SetActive(false);

        // 추가사항 - GameStart 버튼 비활성화
        gameStartButtonObject.interactable = false;
    }

    // Item 버튼 클릭 시
    private void AbilityButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select);
        ItemPage.SetActive(true);
    }

    // Character Page 뒤로가기 버튼 클릭 시
    // 1. CharacterPage_OptionButtonClicked() 호출됐을 때
    // 2. CharacterPage_OptionButtonClicked() 호출되지 않을 때(아무것도 선택 x)
    private void CBackButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select); 
        //CharacterPage.SetActive(false);

        if(isCharacterSelect) // Character 선택 후 스토리 패널 표시 돼있는 경우
        {
            CharacterExplainGroup.SetActive(false);
            isCharacterSelect = false;
        }
        else // 캐릭터 선택 안되고 MainPage로 갈 상황인 경우
        {

            CharacterPage.SetActive(false);
            isCharacterSelect = false;
        }

        
    }

    // Item Page 뒤로가기 버튼 클릭 시
    private void ItemPageBackButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select);
        ItemPage.SetActive(false);
    }

    // Option Page 뒤로가기 버튼 클릭 시
    private void SettingPageBackButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioMaßnager.Sfx.Select);
        SettingPage.SetActive(false);

        // 비활성화했던 버튼들 다시 활성화
        CharacterButtonObject.enabled = true;
        ExitButtonObject.enabled = true;
    }

    // Main Page에 있는 Option 버튼 클릭 시
    private void Main_OptionButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select);
        SettingPage.SetActive(true);

        // 다른 버튼들 비활성화
        CharacterButtonObject.enabled = false; // Character 버튼 비활성화
        ExitButtonObject.enabled = false; // Exit 버튼 비활성화
    }

    private void CharacterPage_OptionButtonClicked()
    {
         // 뭐지 ? 왜 Inspector창에서 한 것처럼 Button 자체가 꺼졌다 켜지지 ?? Button의 interactable이 꺼졌다 켜져야하는...
        gameStartButtonObject.interactable = false; 
    }

    // 추가적으로 해야할 상황
    // CharacterPage의 옵션에서 뒤로가기 버튼 누를 때 gameStartButtonObject interactable을 true로 변경할 것!
    // CharacterPage의 뒤로가기 버튼, 상황별 코딩
}
