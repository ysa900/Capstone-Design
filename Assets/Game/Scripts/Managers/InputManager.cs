using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
    private static InputManager _instance;

    // GameOver_GoTOLobby 버튼
    public UnityEngine.UI.Button GVGoToLobbyButtonObject;

    // GameOver_Restart 버튼
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

    // GotoNextScene 버튼
    public UnityEngine.UI.Button GoToNextSceneButtonObject;

    // Option 관련 버튼
    public UnityEngine.UI.Button OptionButtonObject; // 프레이 화면의 Option 버튼


    // GameManager에게 정보 전달을 하기 위한 Delegate들
    public delegate void OnPauseButtonClicked();
    public OnPauseButtonClicked onPauseButtonClicked;

    public delegate void OnPlayButtonClicked();
    public OnPlayButtonClicked onPlayButtonClicked;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제한다.
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
        // 아래의 함수를 사용하여 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않는다.
        DontDestroyOnLoad(gameObject);
    }

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

        // GoToNextScene 버튼 눌렀을 때
        UnityEngine.UI.Button GoToNextSceneButton = GoToNextSceneButtonObject.GetComponent<UnityEngine.UI.Button>();
        GoToNextSceneButton.onClick.AddListener(goToNextSceneButtonClicked);

        // Play 버튼 눌렀을 때
        UnityEngine.UI.Button PlayButton = PlayButtonObject.GetComponent<UnityEngine.UI.Button>();
        PlayButton.onClick.AddListener(PlayButtonClicked);

        // Option 버튼 눌렀을 때
        UnityEngine.UI.Button OptionButton = OptionButtonObject.GetComponent<UnityEngine.UI.Button>();
        OptionButton.onClick.AddListener(OptionButtonClicked);

    }

    // RestartButton이 눌렀을 때
    private void RestartButtonClicked()
    {
        GameAudioManager.instance.bgmPlayer.Stop(); // 현재 BGM 종료

        SceneManager.LoadScene("Stage1"); // Stage1 으로 돌아가기 
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
        if (!GameManager.instance.isSettingPageOn) // 설정창 켜지지 않은 상태에서 누르기
        {
            PauseButtonObject.interactable = false;
            // 인게임 중
            if (!GameManager.instance.isPausePageOn && !GameManager.instance.isClearPageOn && !GameManager.instance.isSkillSelectPageOn && !GameManager.instance.isDeadPageOn)
            {
                GameManager.instance.isSettingPageOn = true;
                Time.timeScale = 0; // 화면 멈추기
                GameManager.instance.SettingPageObject.SetActive(true);
            }
            else // 나머지 상황: Pause, 레벨업, 클리어, 게임 오버
            {
                GameManager.instance.isSettingPageOn = true;
                GameManager.instance.SettingPageObject.SetActive(true);
            }
        }
        else // 설정창 켜진 상태에서 한번 더 누르기
        {
            // 인게임 중
            if (!GameManager.instance.isPausePageOn && !GameManager.instance.isClearPageOn && !GameManager.instance.isSkillSelectPageOn && !GameManager.instance.isDeadPageOn)
            {
                Time.timeScale = 1; // 화면 풀기
                GameManager.instance.isSettingPageOn = false;
                PauseButtonObject.interactable = true;
            }
            else if (GameManager.instance.isPausePageOn && GameManager.instance.isSettingPageOn) // Pause 화면
            {
                GameManager.instance.isSettingPageOn = false;
                PauseButtonObject.interactable = true;
            }
            else if (GameManager.instance.isClearPageOn && GameManager.instance.isSettingPageOn) // Clear 화면
            {
                GameManager.instance.isSettingPageOn = false;
                PauseButtonObject.interactable = false;
            }
            else if (GameManager.instance.isDeadPageOn && GameManager.instance.isSettingPageOn) // 게임 오버
            {
                GameManager.instance.isSettingPageOn = false;
                PauseButtonObject.interactable = false;
            }

            GameManager.instance.SettingPageObject.SetActive(false);
        }

    }

    private void goToNextSceneButtonClicked()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Stage1":
                SceneManager.LoadScene("Splash2");
                break;
            case "Stage2":
                SceneManager.LoadScene("Splash3");
                break;
        }
    }
}