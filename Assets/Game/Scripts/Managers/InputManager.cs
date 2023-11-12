using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager: MonoBehaviour
{
    // GameoVer_GoTOLobby 버튼
    public UnityEngine.UI.Button GVGoToLobbyButtonObject;

    // GameoVer_Restart 버튼
    public UnityEngine.UI.Button GVRestartButtonObject;

    // Pause 버튼
    public UnityEngine.UI.Button PauseButtonObject;

    // Pause_GoTOLobby 버튼
    public UnityEngine.UI.Button PGoToLobbyButtonObject;

    // Pause_Restart 버튼
    public UnityEngine.UI.Button PRestartButtonObject;

    // Play 버튼
    public UnityEngine.UI.Button PlayButtonObject;



    // GameManager에게 정보 전달을 하기 위한 Delegate들
    public delegate void OnPauseButtonClicked();
    public OnPauseButtonClicked onPauseButtonClicked;

    public delegate void OnPlayButtonClicked();
    public OnPlayButtonClicked onPlayButtonClicked;

    // Start is called before the first frame update
    void Start()
    {
        // GameoVer_Restart 버튼 눌렀을 때
        UnityEngine.UI.Button GVRestartButton = GVRestartButtonObject.GetComponent<UnityEngine.UI.Button>();
        GVRestartButton.onClick.AddListener(RestartButtonClicked);

        // GameoVer_GoTOLobby 버튼 눌렀을 때
        UnityEngine.UI.Button GVGoToLobbyButton = GVGoToLobbyButtonObject.GetComponent<UnityEngine.UI.Button>();
        GVGoToLobbyButton.onClick.AddListener(goToLobbyButtonClicked);

        // Pause 버튼 눌렀을 때
        UnityEngine.UI.Button PauseButton = PauseButtonObject.GetComponent<UnityEngine.UI.Button>();
        PauseButton.onClick.AddListener(PauseButtonClicked);

        // Pause_Restart 버튼 눌렀을 때
        UnityEngine.UI.Button PRestartButto = PRestartButtonObject.GetComponent<UnityEngine.UI.Button>();
        PRestartButto.onClick.AddListener(RestartButtonClicked);

        // Pause_GoTOLobby 버튼 눌렀을 때
        UnityEngine.UI.Button PGoToLobbyButton = PGoToLobbyButtonObject.GetComponent<UnityEngine.UI.Button>();
        PGoToLobbyButton.onClick.AddListener(goToLobbyButtonClicked);

        // Play 버튼 눌렀을 때
        UnityEngine.UI.Button PlayButton = PlayButtonObject.GetComponent<UnityEngine.UI.Button>();
        PlayButton.onClick.AddListener(PlayButtonClicked);
    }

    // RestartButton이 눌렀을 때
    private void RestartButtonClicked()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); // 버튼 선택 시 효과음
        SceneManager.LoadScene("Game");
        Time.timeScale = 1;
    }

    // goToLobbyButton이 눌렀을 때
    private void goToLobbyButtonClicked()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); // 버튼 선택 시 효과음
        SceneManager.LoadScene("Lobby");
        Time.timeScale = 1;
    }

    // PauseButton이 눌렀을 때
    private void PauseButtonClicked()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); // 버튼 선택 시 효과음
        if (Time.timeScale == 0) // Pause 누른 상태에서 한번 더 누르면 Pause 풀리게 하려고
            PlayButtonClicked();
        else
        {
            Time.timeScale = 0;
            onPauseButtonClicked(); // delegate 호출
        }
    }

    // PlayButton이 눌렀을 때
    private void PlayButtonClicked()
    {
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select); // 버튼 선택 시 효과음
        Time.timeScale = 1;
        onPlayButtonClicked(); // delegate 호출
    }
}
