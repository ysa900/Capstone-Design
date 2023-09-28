using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    // gameStart ��ư
    public UnityEngine.UI.Button gameStartButtonObject;

    // Character ��ư
    public UnityEngine.UI.Button CharacterButtonObject;

    // Ability ��ư
    public UnityEngine.UI.Button AbilityButtonObject;

    // CharacterPage Back ��ư
    public UnityEngine.UI.Button CBackButtonObject;

    // AbilityrPage Back ��ư
    public UnityEngine.UI.Button ABackButtonObject;

    // ĳ���� ������ ������Ʈ
    public GameObject CharacterPage;

    // �ɷ�ġ ������ ������Ʈ
    public GameObject AbilityPage;

    // Start is called before the first frame update
    void Start()
    {
        // ���� �� ��Ȱ��ȭ
        CharacterPage.SetActive(false);
        AbilityPage.SetActive(false);

        // Restart ��ư ������ ��
        UnityEngine.UI.Button gameStartButton = gameStartButtonObject.GetComponent<UnityEngine.UI.Button>();
        gameStartButton.onClick.AddListener(GameStartButtonClicked);

        // Character ��ư ������ ��
        UnityEngine.UI.Button CharacterButton = CharacterButtonObject.GetComponent<UnityEngine.UI.Button>();
        CharacterButton.onClick.AddListener(CharacterButtonClicked);

        // Ability ��ư ������ ��
        UnityEngine.UI.Button AbilityButton = AbilityButtonObject.GetComponent<UnityEngine.UI.Button>();
        AbilityButton.onClick.AddListener(AbilityButtonClicked);

        // ĳ���� ������ �ڷΰ��� ��ư ������ ��
        UnityEngine.UI.Button CBackButton = CBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        CBackButton.onClick.AddListener(CBackButtonClicked);

        // �ɷ�ġ ������ �ڷΰ��� ��ư ������ ��
        UnityEngine.UI.Button ABackButton = ABackButtonObject.GetComponent<UnityEngine.UI.Button>();
        ABackButton.onClick.AddListener(ABackButtonClicked);
    }

    // ���� ���� ��ư Ŭ����
    private void GameStartButtonClicked()
    {
        SceneManager.LoadScene("Game");
    }

    // ĳ���� ��ư Ŭ����
    private void CharacterButtonClicked()
    {
        CharacterPage.SetActive(true);
    }

    // ĳ���� ������ �ڷΰ��� ��ư Ŭ����
    private void CBackButtonClicked()
    {
        CharacterPage.SetActive(false);
    }

    // �ɷ�ġ ��ư Ŭ����
    private void AbilityButtonClicked()
    {
        AbilityPage.SetActive(true);
    }

    // �ɷ�ġ ������ �ڷΰ��� ��ư Ŭ����
    private void ABackButtonClicked()
    {
        AbilityPage.SetActive(false);
    }
}
