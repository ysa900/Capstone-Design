using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager: MonoBehaviour
{
    // GameoVer_GoTOLobby ��ư
    public UnityEngine.UI.Button GVGoToLobbyButtonObject;

    // GameoVer_Restart ��ư
    public UnityEngine.UI.Button GVRestartButtonObject;

    // Pause ��ư
    public UnityEngine.UI.Button PauseButtonObject;

    // Pause_GoTOLobby ��ư
    public UnityEngine.UI.Button PGoToLobbyButtonObject;

    // Pause_Restart ��ư
    public UnityEngine.UI.Button PRestartButtonObject;

    // Play ��ư
    public UnityEngine.UI.Button PlayButtonObject;



    // GameManager���� ���� ������ �ϱ� ���� Delegate��
    public delegate void OnPauseButtonClicked();
    public OnPauseButtonClicked onPauseButtonClicked;

    public delegate void OnPlayButtonClicked();
    public OnPlayButtonClicked onPlayButtonClicked;

    // Start is called before the first frame update
    void Start()
    {
        // GameoVer_Restart ��ư ������ ��
        UnityEngine.UI.Button GVRestartButton = GVRestartButtonObject.GetComponent<UnityEngine.UI.Button>();
        GVRestartButton.onClick.AddListener(RestartButtonClicked);

        // GameoVer_GoTOLobby ��ư ������ ��
        UnityEngine.UI.Button GVGoToLobbyButton = GVGoToLobbyButtonObject.GetComponent<UnityEngine.UI.Button>();
        GVGoToLobbyButton.onClick.AddListener(goToLobbyButtonClicked);

        // Pause ��ư ������ ��
        UnityEngine.UI.Button PauseButton = PauseButtonObject.GetComponent<UnityEngine.UI.Button>();
        PauseButton.onClick.AddListener(PauseButtonClicked);

        // Pause_Restart ��ư ������ ��
        UnityEngine.UI.Button PRestartButto = PRestartButtonObject.GetComponent<UnityEngine.UI.Button>();
        PRestartButto.onClick.AddListener(RestartButtonClicked);

        // Pause_GoTOLobby ��ư ������ ��
        UnityEngine.UI.Button PGoToLobbyButton = PGoToLobbyButtonObject.GetComponent<UnityEngine.UI.Button>();
        PGoToLobbyButton.onClick.AddListener(goToLobbyButtonClicked);

        // Play ��ư ������ ��
        UnityEngine.UI.Button PlayButton = PlayButtonObject.GetComponent<UnityEngine.UI.Button>();
        PlayButton.onClick.AddListener(PlayButtonClicked);
    }

    // RestartButton�� ������ ��
    private void RestartButtonClicked()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); // ��ư ���� �� ȿ����
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }

    // goToLobbyButton�� ������ ��
    private void goToLobbyButtonClicked()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); // ��ư ���� �� ȿ����
        SceneManager.LoadScene("Lobby");
        Time.timeScale = 1;
    }

    // PauseButton�� ������ ��
    private void PauseButtonClicked()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); // ��ư ���� �� ȿ����
        if (Time.timeScale == 0) // Pause ���� ���¿��� �ѹ� �� ������ Pause Ǯ���� �Ϸ���
            PlayButtonClicked();
        else
        {
            Time.timeScale = 0;
            onPauseButtonClicked(); // delegate ȣ��
        }
    }

    // PlayButton�� ������ ��
    private void PlayButtonClicked()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); // ��ư ���� �� ȿ����
        Time.timeScale = 1;
        onPlayButtonClicked(); // delegate ȣ��
    }
}
