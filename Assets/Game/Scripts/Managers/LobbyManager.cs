using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private LobbyAudioManager lobbyAudioManager;

    // gameStart ��ư
    public UnityEngine.UI.Button gameStartButtonObject;

    // Character ��ư
    public UnityEngine.UI.Button CharacterButtonObject;

    // Item ��ư
    public UnityEngine.UI.Button ItemButtonObject;

    // Character Page Back ��ư
    public UnityEngine.UI.Button CharacterPageBackButtonObject;

    // Item Page Back ��ư
    public UnityEngine.UI.Button ItemPageBackButtonObject;

    // Setting Page Back 버튼
    public UnityEngine.UI.Button SettingPageBackButtonObject;

    // Exit ��ư
    public UnityEngine.UI.Button ExitButtonObject;

    // Option 버튼
    public UnityEngine.UI.Button OptionButtonObject;

    // Character Page ������Ʈ
    public GameObject CharacterPage;

    // Item Page ������Ʈ
    public GameObject ItemPage;

    // Setting Page
    public GameObject SettingPage;

    // ĳ���� ���� �� ���丮 �г�
    public GameObject CharacterExplainGroup;

    // ĳ���� ����
    public GameObject SelectAssassin; // Assasin
    public GameObject SelectMage; // Mage
    public GameObject SelectWarrior; // Warrior

    private void Awake()
    {
        lobbyAudioManager = FindAnyObjectByType<LobbyAudioManager>();
    }

    private void Start()
    {
        //lobbyAudioManager.PlayBGM(true);
        // ���� �� ��Ȱ��ȭ
        CharacterPage.SetActive(false);
        ItemPage.SetActive(false);

        // Restart ��ư ������ ��
        UnityEngine.UI.Button gameStartButton = gameStartButtonObject.GetComponent<UnityEngine.UI.Button>();
        gameStartButton.onClick.AddListener(GameStartButtonClicked);

        // Exit ��ư ������ ��
        UnityEngine.UI.Button ExitButton = ExitButtonObject.GetComponent<UnityEngine.UI.Button>();
        ExitButton.onClick.AddListener(ExitButtonClicked);

        // Character ��ư ������ ��
        UnityEngine.UI.Button CharacterButton = CharacterButtonObject.GetComponent<UnityEngine.UI.Button>();
        CharacterButton.onClick.AddListener(CharacterButtonClicked);

        // Item ��ư ������ ��
        UnityEngine.UI.Button AbilityButton = ItemButtonObject.GetComponent<UnityEngine.UI.Button>();
        AbilityButton.onClick.AddListener(AbilityButtonClicked);

        // Character Page �ڷΰ��� ��ư ������ ��
        UnityEngine.UI.Button CharacterPageBackButton = CharacterPageBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        CharacterPageBackButton.onClick.AddListener(CBackButtonClicked);

        // Item Page �ڷΰ��� ��ư ������ ��
        UnityEngine.UI.Button ItemPageBackButton = ItemPageBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        ItemPageBackButton.onClick.AddListener(ItemPageBackButtonClicked);

        // Option 뒤로가기 버튼 초기화
        UnityEngine.UI.Button SettingPageBackButton = SettingPageBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        SettingPageBackButton.onClick.AddListener(SettingPageBackButtonClicked);

        // Option 버튼 초기화
        UnityEngine.UI.Button OptionButton = OptionButtonObject.GetComponent<UnityEngine.UI.Button>();
        OptionButton.onClick.AddListener(OptionButtonClicked);

        // Mage ���� ��
        UnityEngine.UI.Button SelectMageButton = SelectMage.GetComponent<UnityEngine.UI.Button>();
        SelectMageButton.onClick.AddListener(SelectMageButtonClicked);
    }

    // Exit ��ư Ŭ����
    private void ExitButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select); // ��ư Ŭ�� �� ȿ����

        // ����Ƽ �����Ϳ��� ���� �÷��� ���� ���� #if Ű���� ���
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit(); // ���� ����
        #endif
    }
    
    // Mage ���ý�
    private void SelectMageButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select); // ��ư Ŭ�� �� ȿ����
        CharacterExplainGroup.SetActive(true);
        // GameStart ��ư Ȱ��ȭ ??

    }

    // GameStart ��ư Ŭ����
    private void GameStartButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select); // ��ư Ŭ�� �� ȿ����
        SceneManager.LoadScene("Game"); // Game �� �ҷ�����
    }

    // Character ��ư Ŭ����
    private void CharacterButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select); // ��ư Ŭ�� �� ȿ����
        CharacterPage.SetActive(true);
        CharacterExplainGroup.SetActive(false);
        // GameStart ��ư ��Ȱ��ȭ ??
    }

    // Item ��ư Ŭ����
    private void AbilityButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select);
        ItemPage.SetActive(true);
    }

    // Character Page �ڷΰ��� ��ư Ŭ����
    private void CBackButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select); 
        CharacterPage.SetActive(false);
    }

    // Item Page �ڷΰ��� ��ư Ŭ����
    private void ItemPageBackButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select);
        ItemPage.SetActive(false);
    }

    // Option Page 뒤로가기
    private void SettingPageBackButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioMaßnager.Sfx.Select);
        SettingPage.SetActive(false);

        // 비활성화했던 버튼들 다시 활성화
        CharacterButtonObject.interactable = true;
        ExitButtonObject.interactable = true;
    }

    private void OptionButtonClicked()
    {
        //lobbyAudioManager.PlaySfx(LobbyAudioManager.Sfx.Select);
        SettingPage.SetActive(true);

        // 다른 버튼들 비활성화
        CharacterButtonObject.enabled = false; // Character 버튼 비활성화
        ExitButtonObject.enabled = false; // Exit 버튼 비활성화

    }
}
