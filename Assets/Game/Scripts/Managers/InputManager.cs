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

    // Option 관련 버튼
    public UnityEngine.UI.Button OptionButtonObject; // 플레이 화면의 Option 버튼
    // Option Back Button 관련 버튼
    public UnityEngine.UI.Button OptionBackbuttonObject; // Setting 창 Back Button


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

        // Play 버튼 눌렀을 때
        UnityEngine.UI.Button PlayButton = PlayButtonObject.GetComponent<UnityEngine.UI.Button>();
        PlayButton.onClick.AddListener(PlayButtonClicked);

        // Option 버튼 눌렀을 때
        UnityEngine.UI.Button OptionButton = OptionButtonObject.GetComponent<UnityEngine.UI.Button>();
        OptionButton.onClick.AddListener(OptionButtonClicked);

        UnityEngine.UI.Button OptionBackbutton = OptionBackbuttonObject.GetComponent<UnityEngine.UI.Button>();
        OptionBackbuttonObject.onClick.AddListener(OptionBackButtonClicked);
    }

    // RestartButton이 눌렀을 때
    private void RestartButtonClicked()
    {
        GameAudioManager.instance.bgmPlayer.Stop(); // 현재 BGM 종료

        // if(GameManager.instance.gameOverObject.activeSelf)
        //     GameManager.instance.gameOverObject.SetActive(false);
        // if(GameManager.instance.pauseObject.activeSelf)
        //     GameManager.instance.pauseObject.SetActive(false);
        
        SceneManager.LoadScene("Stage1"); // Stage1 으로 돌아가기 
    }

    // goToLobbyButton이 눌렀을 때
    private void goToLobbyButtonClicked()
    {
        GameAudioManager.instance.bgmPlayer.Stop(); // 현재 BGM 종료

        // if(GameManager.instance.gameOverObject.activeSelf)
        //     GameManager.instance.gameOverObject.SetActive(false);
        // if(GameManager.instance.pauseObject.activeSelf)
        //     GameManager.instance.pauseObject.SetActive(false);

        SceneManager.LoadScene("Lobby");
        Time.timeScale = 1;
    }

    // PauseButton이 눌렀을 때
    private void PauseButtonClicked()
    {
        onPauseButtonClicked(); // delegate 호출
    }

    // PlayButton이 눌렀을 때
    private void PlayButtonClicked()
    {
        onPlayButtonClicked(); // delegate 호출
    }

    private void OptionButtonClicked()
    {
        // Setting창 안켜져있을 때 누르면
        if (!GameManager.instance.isSettingPageOn)
        {
            if (!GameManager.instance.isGameOver && !GameManager.instance.isDeadPageOn && !GameManager.instance.isSkillSelectPageOn)
            {
                Time.timeScale = 0; // 인게임 플레이 중에만 멈추기
                PauseButtonObject.interactable = false;  // 인게임 플레이 중에만 Pause 버튼 비활성화
            }    

            GameManager.instance.isSettingPageOn = true;
            GameManager.instance.SettingPageObject.SetActive(true);
        }
        // Setting창 켜져있을 때 누른다면
        else
        {   
            // Pause버튼이 비활성화돼있는 상황인 경우
            if(!GameManager.instance.isDeadPageOn && !GameManager.instance.isSkillSelectPageOn)
            {
                if (!GameManager.instance.pauseObject.activeSelf || !GameManager.instance.gameClearObject.activeSelf)
                    if (!GameManager.instance.pauseObject.activeSelf)
                        Time.timeScale = 1;
                PauseButtonObject.interactable = true;  // 인게임 플레이 중에만 Pause 버튼 비활성화
            }

            GameManager.instance.isSettingPageOn = false;
            GameManager.instance.SettingPageObject.SetActive(false);

        }        
    }

    private void OptionBackButtonClicked()
    {
        if(GameManager.instance.isSettingPageOn)
        {
            // Pause버튼이 비활성화돼있는 상황인 경우
            if(!GameManager.instance.isDeadPageOn && !GameManager.instance.isSkillSelectPageOn)
            {
                if (!GameManager.instance.pauseObject.activeSelf)
                    Time.timeScale = 1;
                PauseButtonObject.interactable = true;
            }

            GameManager.instance.isSettingPageOn = false;
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