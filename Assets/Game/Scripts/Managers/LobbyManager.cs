using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ĳ���� ���� �� �ڷ� ���� ��ư ������ �ٽ� ĳ���� ����â���� ��� ��������..?
// ĳ���� ����â���� GameStart��ư On/Off �ϴ� ��...
// �̰� �ΰ� �𸣰ھ�


public class LobbyManager : MonoBehaviour
{
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

    // Exit ��ư
    public UnityEngine.UI.Button ExitButtonObject;

    // Character Page ������Ʈ
    public GameObject CharacterPage;

    // Item Page ������Ʈ
    public GameObject ItemPage;

    // ĳ���� ���� �� ���丮 �г�
    public GameObject CharacterExplainGroup;

    // ĳ���� ����
    public GameObject SelectAssassin; // Assasin
    public GameObject SelectMage; // Mage
    public GameObject SelectWarrior; // Warrior

    // Start is called before the first frame update
    void Start()
    {
        // ���� �� ��Ȱ��ȭ
        CharacterPage.SetActive(false);
        ItemPage.SetActive(false);

        // Restart ��ư ������ ��
        UnityEngine.UI.Button gameStartButton = gameStartButtonObject.GetComponent<UnityEngine.UI.Button>();
        gameStartButton.onClick.AddListener(GameStartButtonClicked);

        // Exit ��ư ������ ��
        UnityEngine.UI.Button exitButton = ExitButtonObject.GetComponent<UnityEngine.UI.Button>();
        exitButton.onClick.AddListener(ExitButtonClicked);

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

        // Mage ���� ��
        UnityEngine.UI.Button SelectMageButton = SelectMage.GetComponent<UnityEngine.UI.Button>();
        SelectMageButton.onClick.AddListener(SelectMageButtonClicked);
    }

    // Exit ��ư Ŭ����
    private void ExitButtonClicked()
    {
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
        CharacterExplainGroup.SetActive(true);
        // GameStart ��ư Ȱ��ȭ ??

    }

    // GameStart ��ư Ŭ����
    private void GameStartButtonClicked()
    {
        SceneManager.LoadScene("Game"); // Game �� �ҷ�����
    }

    // Character ��ư Ŭ����
    private void CharacterButtonClicked()
    {
        CharacterPage.SetActive(true);
        CharacterExplainGroup.SetActive(false);
        // GameStart ��ư ��Ȱ��ȭ ??

    }

    // Item ��ư Ŭ����
    private void AbilityButtonClicked()
    {
        ItemPage.SetActive(true);
    }

    // Character Page �ڷΰ��� ��ư Ŭ����
    private void CBackButtonClicked()
    {
        CharacterPage.SetActive(false);
    }

    // Item page �ڷΰ��� ��ư Ŭ����
    private void ItemPageBackButtonClicked()
    {
        ItemPage.SetActive(false);
    }
}
