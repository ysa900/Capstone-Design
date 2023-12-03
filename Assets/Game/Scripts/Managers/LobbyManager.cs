using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    // Exit 버튼
    public UnityEngine.UI.Button ExitButtonObject;

    // Character Page 오브젝트
    public GameObject CharacterPage;

    // Item Page 오브젝트
    public GameObject ItemPage;

    // 캐릭터 선택 및 스토리 패널
    public GameObject CharacterExplainGroup;

    // 캐릭터 선택
    public GameObject SelectAssassin; // Assasin
    public GameObject SelectMage; // Mage
    public GameObject SelectWarrior; // Warrior

    private void Awake()
    {
        lobbyAudioManager = FindAnyObjectByType<LobbyAudioManager>();
    }

    private void Start()
    {
        lobbyAudioManager.PlayBGM(true);
        // 시작 시 비활성화
        CharacterPage.SetActive(false);
        ItemPage.SetActive(false);

        // Restart 버튼 눌렀을 때
        UnityEngine.UI.Button gameStartButton = gameStartButtonObject.GetComponent<UnityEngine.UI.Button>();
        gameStartButton.onClick.AddListener(GameStartButtonClicked);

        // Exit 버튼 눌렀을 때
        UnityEngine.UI.Button exitButton = ExitButtonObject.GetComponent<UnityEngine.UI.Button>();
        exitButton.onClick.AddListener(ExitButtonClicked);

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

        // Mage 선택 시
        UnityEngine.UI.Button SelectMageButton = SelectMage.GetComponent<UnityEngine.UI.Button>();
        SelectMageButton.onClick.AddListener(SelectMageButtonClicked);
    }

    // Exit 버튼 클릭시
    private void ExitButtonClicked()
    {
        lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select); // 버튼 클릭 시 효과음

        // 유니티 에디터에서 게임 플레이 종료 위한 #if 키워드 사용
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit(); // 게임 종료
        #endif
    }
    
    // Mage 선택시
    private void SelectMageButtonClicked()
    {
        lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select); // 버튼 클릭 시 효과음
        CharacterExplainGroup.SetActive(true);
        // GameStart 버튼 활성화 ??

    }

    // GameStart 버튼 클릭시
    private void GameStartButtonClicked()
    {
        lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select); // 버튼 클릭 시 효과음
        SceneManager.LoadScene("Game"); // Game 씬 불러오기
    }

    // Character 버튼 클릭시
    private void CharacterButtonClicked()
    {
        lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select); // 버튼 클릭 시 효과음
        CharacterPage.SetActive(true);
        CharacterExplainGroup.SetActive(false);
        // GameStart 버튼 비활성화 ??
    }

    // Item 버튼 클릭시
    private void AbilityButtonClicked()
    {
        lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select);
        ItemPage.SetActive(true);
    }

    // Character Page 뒤로가기 버튼 클릭시
    private void CBackButtonClicked()
    {
        lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select); 
        CharacterPage.SetActive(false);
    }

    // Item page 뒤로가기 버튼 클릭시
    private void ItemPageBackButtonClicked()
    {
        lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select);
        ItemPage.SetActive(false);
    }
}
