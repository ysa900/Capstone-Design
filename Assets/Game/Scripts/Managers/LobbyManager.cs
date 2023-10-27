using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    // gameStart ��ư
    public UnityEngine.UI.Button gameStartButtonObject;

    // Character ��ư
    public UnityEngine.UI.Button CharacterButtonObject;

    // Item ��ư
    public UnityEngine.UI.Button ItemButtonObject;

    // Character Page Back ��ư
    public UnityEngine.UI.Button ABackButtonObject;

    // Item Page Back ��ư
    public UnityEngine.UI.Button BBackButtonObject;

    // Character Page ������Ʈ
    public GameObject CharacterPage;

    // Item Page ������Ʈ
    public GameObject ItemPage;

    // Start is called before the first frame update
    void Start()
    {
        // ���� �� ��Ȱ��ȭ
        CharacterPage.SetActive(false);
        ItemPage.SetActive(false);

        // Restart ��ư ������ ��
        UnityEngine.UI.Button gameStartButton = gameStartButtonObject.GetComponent<UnityEngine.UI.Button>();
        gameStartButton.onClick.AddListener(GameStartButtonClicked);

        // Character ��ư ������ ��
        UnityEngine.UI.Button CharacterButton = CharacterButtonObject.GetComponent<UnityEngine.UI.Button>();
        CharacterButton.onClick.AddListener(CharacterButtonClicked);

        // Item ��ư ������ ��
        UnityEngine.UI.Button AbilityButton = ItemButtonObject.GetComponent<UnityEngine.UI.Button>();
        AbilityButton.onClick.AddListener(AbilityButtonClicked);

        // Character Page �ڷΰ��� ��ư ������ ��
        UnityEngine.UI.Button CBackButton = ABackButtonObject.GetComponent<UnityEngine.UI.Button>();
        CBackButton.onClick.AddListener(CBackButtonClicked);

        // Item Page �ڷΰ��� ��ư ������ ��
        UnityEngine.UI.Button ABackButton = BBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        ABackButton.onClick.AddListener(ABackButtonClicked);
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
    private void ABackButtonClicked()
    {
        ItemPage.SetActive(false);
    }
}
