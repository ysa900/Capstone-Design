using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    // GameoVer_GoTOLobby 버튼
    public UnityEngine.UI.Button GVGoToLobbyButtonObject;

    // GameoVer_Restart 버튼
    public UnityEngine.UI.Button GVRestartButtonObject;

    // GameClear_GoTOLobby 버튼
    public UnityEngine.UI.Button GCGoToLobbyButtonObject;

    // Pause 버튼
    public UnityEngine.UI.Button PauseButtonObject;

    // Pause_GoTOLobby 버튼
    public UnityEngine.UI.Button PGoToLobbyButtonObject;

    // Pause_Restart 버튼
    public UnityEngine.UI.Button PRestartButtonObject;

    // Play 버튼
    public UnityEngine.UI.Button PlayButtonObject;

    // Option 관련 버튼
    public UnityEngine.UI.Button OptionButtonObject; // 프레이 화면의 Option 버튼
    public UnityEngine.UI.Button SettingPageBackButtonObject; // Setting Page(옵션 창)의 Back 버튼


    // GameManager에게 정보 전달을 하기 위한 Delegate들
    public delegate void OnPauseButtonClicked();
    public OnPauseButtonClicked onPauseButtonClicked;

    public delegate void OnPlayButtonClicked();
    public OnPlayButtonClicked onPlayButtonClicked;

    // Start is called before the first frame update
    void Start()
    {
        // GameOver_Restart 버튼 눌렀을 때
        UnityEngine.UI.Button GVRestartButton = GVRestartButtonObject.GetComponent<UnityEngine.UI.Button>();
        GVRestartButton.onClick.AddListener(RestartButtonClicked);

        // GameOver_GoTOLobby 버튼 눌렀을 때
        UnityEngine.UI.Button GVGoToLobbyButton = GVGoToLobbyButtonObject.GetComponent<UnityEngine.UI.Button>();
        GVGoToLobbyButton.onClick.AddListener(goToLobbyButtonClicked);

        // GameClear_GoTOLobby 버튼 눌렀을 때
        UnityEngine.UI.Button GCGoToLobbyButton = GCGoToLobbyButtonObject.GetComponent<UnityEngine.UI.Button>();
        GCGoToLobbyButton.onClick.AddListener(goToLobbyButtonClicked);

        // Pause 버튼 눌렀을 때
        UnityEngine.UI.Button PauseButton = PauseButtonObject.GetComponent<UnityEngine.UI.Button>();
        PauseButton.onClick.AddListener(PauseButtonClicked);

        // Pause_Restart 버튼 눌렀을 때
        UnityEngine.UI.Button PRestartButton = PRestartButtonObject.GetComponent<UnityEngine.UI.Button>();
        PRestartButton.onClick.AddListener(RestartButtonClicked);

        // Pause_GoTOLobby 버튼 눌렀을 때
        UnityEngine.UI.Button PGoToLobbyButton = PGoToLobbyButtonObject.GetComponent<UnityEngine.UI.Button>();
        PGoToLobbyButton.onClick.AddListener(goToLobbyButtonClicked);

        // Play 버튼 눌렀을 때
        UnityEngine.UI.Button PlayButton = PlayButtonObject.GetComponent<UnityEngine.UI.Button>();
        PlayButton.onClick.AddListener(PlayButtonClicked);

        // Option 버튼 눌렀을 때
        UnityEngine.UI.Button OptionButton = OptionButtonObject.GetComponent<UnityEngine.UI.Button>();
        OptionButton.onClick.AddListener(OptionButtonClicked);

        // SettingPage의 BackButton 눌렀을 때
        UnityEngine.UI.Button SettingPageBackButton = SettingPageBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        SettingPageBackButton.onClick.AddListener(SettingPageBackButtonClicked);
    }

    // RestartButton이 눌렀을 때
    private void RestartButtonClicked()
    {
        GameAudioManager.instance.bgmPlayer.Stop(); // 현재 BGM 종료

<<<<<<< HEAD
        SceneManager.LoadScene("Stage1"); // Stage1 으로 돌아가기(해당 씬 Load), 현재는 "Game" 씬 
=======
        SceneManager.LoadScene("Game"); // Stage1 으로 돌아가기(해당 씬 Load), 현재는 "Game" 씬 
>>>>>>> fb9e122ace47cd59b98368c2d381069dfdb7632d
        Time.timeScale = 1;
    }

    // goToLobbyButton이 눌렀을 때
    private void goToLobbyButtonClicked()
    {
        GameAudioManager.instance.bgmPlayer.Stop(); // 현재 BGM 종료

        SceneManager.LoadScene("Lobby");
        Time.timeScale = 1;
    }
    
    // PauseButton이 눌렀을 때
    private void PauseButtonClicked()
    {
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
        Time.timeScale = 1;
        onPlayButtonClicked(); // delegate 호출
    }


    private void OptionButtonClicked()
    {
        Time.timeScale = 0;
<<<<<<< HEAD
        //GameManager.instance.HpBarObject.SetActive(false);
=======
        GameManager.instance.HpBarObject.SetActive(false);
>>>>>>> fb9e122ace47cd59b98368c2d381069dfdb7632d
        GameManager.instance.SettingPageObject.SetActive(true);
        OptionButtonObject.interactable = false;
    }

    private void SettingPageBackButtonClicked()
    {
        Time.timeScale = 1;
<<<<<<< HEAD
        //GameManager.instance.HpBarObject.SetActive(true);
=======
        GameManager.instance.HpBarObject.SetActive(true);
>>>>>>> fb9e122ace47cd59b98368c2d381069dfdb7632d
        GameManager.instance.SettingPageObject.SetActive(false);
        OptionButtonObject.interactable = true;
    }
}