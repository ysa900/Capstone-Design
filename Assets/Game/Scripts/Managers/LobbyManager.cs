using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    // gameStart 버튼
    public UnityEngine.UI.Button gameStartButtonObject;

    // Character 버튼
    public UnityEngine.UI.Button CharacterButtonObject;

    // Ability 버튼
    public UnityEngine.UI.Button AbilityButtonObject;

    // CharacterPage Back 버튼
    public UnityEngine.UI.Button CBackButtonObject;

    // AbilityrPage Back 버튼
    public UnityEngine.UI.Button ABackButtonObject;

    // 캐릭터 페이지 오브젝트
    public GameObject CharacterPage;

    // 능력치 페이지 오브젝트
    public GameObject AbilityPage;

    // Start is called before the first frame update
    void Start()
    {
        // 시작 시 비활성화
        CharacterPage.SetActive(false);
        AbilityPage.SetActive(false);

        // Restart 버튼 눌렀을 때
        UnityEngine.UI.Button gameStartButton = gameStartButtonObject.GetComponent<UnityEngine.UI.Button>();
        gameStartButton.onClick.AddListener(GameStartButtonClicked);

        // Character 버튼 눌렀을 때
        UnityEngine.UI.Button CharacterButton = CharacterButtonObject.GetComponent<UnityEngine.UI.Button>();
        CharacterButton.onClick.AddListener(CharacterButtonClicked);

        // Ability 버튼 눌렀을 때
        UnityEngine.UI.Button AbilityButton = AbilityButtonObject.GetComponent<UnityEngine.UI.Button>();
        AbilityButton.onClick.AddListener(AbilityButtonClicked);

        // 캐릭터 페이지 뒤로가기 버튼 눌렀을 때
        UnityEngine.UI.Button CBackButton = CBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        CBackButton.onClick.AddListener(CBackButtonClicked);

        // 능력치 페이지 뒤로가기 버튼 눌렀을 때
        UnityEngine.UI.Button ABackButton = ABackButtonObject.GetComponent<UnityEngine.UI.Button>();
        ABackButton.onClick.AddListener(ABackButtonClicked);
    }

    // 게임 시작 버튼 클릭시
    private void GameStartButtonClicked()
    {
        SceneManager.LoadScene("Game");
    }

    // 캐릭터 버튼 클릭시
    private void CharacterButtonClicked()
    {
        CharacterPage.SetActive(true);
    }

    // 캐릭터 페이지 뒤로가기 버튼 클릭시
    private void CBackButtonClicked()
    {
        CharacterPage.SetActive(false);
    }

    // 능력치 버튼 클릭시
    private void AbilityButtonClicked()
    {
        AbilityPage.SetActive(true);
    }

    // 능력치 페이지 뒤로가기 버튼 클릭시
    private void ABackButtonClicked()
    {
        AbilityPage.SetActive(false);
    }
}
