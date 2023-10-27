using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    // gameStart 버튼
    public UnityEngine.UI.Button gameStartButtonObject;

    // Character 버튼
    public UnityEngine.UI.Button CharacterButtonObject;

    // Item 버튼
    public UnityEngine.UI.Button ItemButtonObject;

    // Character Page Back 버튼
    public UnityEngine.UI.Button ABackButtonObject;

    // Item Page Back 버튼
    public UnityEngine.UI.Button BBackButtonObject;

    // Character Page 오브젝트
    public GameObject CharacterPage;

    // Item Page 오브젝트
    public GameObject ItemPage;

    // Start is called before the first frame update
    void Start()
    {
        // 시작 시 비활성화
        CharacterPage.SetActive(false);
        ItemPage.SetActive(false);

        // Restart 버튼 눌렀을 때
        UnityEngine.UI.Button gameStartButton = gameStartButtonObject.GetComponent<UnityEngine.UI.Button>();
        gameStartButton.onClick.AddListener(GameStartButtonClicked);

        // Character 버튼 눌렀을 때
        UnityEngine.UI.Button CharacterButton = CharacterButtonObject.GetComponent<UnityEngine.UI.Button>();
        CharacterButton.onClick.AddListener(CharacterButtonClicked);

        // Item 버튼 눌렀을 때
        UnityEngine.UI.Button AbilityButton = ItemButtonObject.GetComponent<UnityEngine.UI.Button>();
        AbilityButton.onClick.AddListener(AbilityButtonClicked);

        // Character Page 뒤로가기 버튼 눌렀을 때
        UnityEngine.UI.Button CBackButton = ABackButtonObject.GetComponent<UnityEngine.UI.Button>();
        CBackButton.onClick.AddListener(CBackButtonClicked);

        // Item Page 뒤로가기 버튼 눌렀을 때
        UnityEngine.UI.Button ABackButton = BBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        ABackButton.onClick.AddListener(ABackButtonClicked);
    }

    // GameStart 버튼 클릭시
    private void GameStartButtonClicked()
    {
        SceneManager.LoadScene("Game"); // Game 씬 불러오기
    }

    // Character 버튼 클릭시
    private void CharacterButtonClicked()
    {
        CharacterPage.SetActive(true);
    }

    // Item 버튼 클릭시
    private void AbilityButtonClicked()
    {
        ItemPage.SetActive(true);
    }

    // Character Page 뒤로가기 버튼 클릭시
    private void CBackButtonClicked()
    {
        CharacterPage.SetActive(false);
    }


    // Item page 뒤로가기 버튼 클릭시
    private void ABackButtonClicked()
    {
        ItemPage.SetActive(false);
    }
}
