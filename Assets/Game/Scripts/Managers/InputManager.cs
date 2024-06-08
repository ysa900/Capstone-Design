using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
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
    public UnityEngine.UI.Button OptionButtonObject; // 플레이 화면의 Option 버튼

    public UnityEngine.UI.Button OptionBackButtonObject; // Setting 창 뒤로가기 버튼


    // GameManager에게 정보 전달을 하기 위한 Delegate들
    public delegate void OnPauseButtonClicked();
    public OnPauseButtonClicked onPauseButtonClicked;

    public delegate void OnPlayButtonClicked();
    public OnPlayButtonClicked onPlayButtonClicked;

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

        // Setting 창 뒤로가기 버튼
        UnityEngine.UI.Button OptionBackButton = OptionBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        OptionBackButton.onClick.AddListener(OptionBackButtonClicked);
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
            // 클리어
            else if (GameManager.instance.isDeadPageOn || GameManager.instance.isClearPageOn && !GameManager.instance.isPausePageOn && !GameManager.instance.isSkillSelectPageOn)
            {
                GameManager.instance.isSettingPageOn = true;
                GameManager.instance.SettingPageObject.SetActive(true);
            }
            // 게임 오버
            else if (GameManager.instance.isGameOver || GameManager.instance.isDeadPageOn && !GameManager.instance.isClearPageOn && !GameManager.instance.isPausePageOn && !GameManager.instance.isSkillSelectPageOn)
            {
                GameManager.instance.isSettingPageOn = true;
                GameManager.instance.SettingPageObject.SetActive(true);
            }
            // Pause
            else if(GameManager.instance.isPausePageOn && !GameManager.instance.isDeadPageOn && !GameManager.instance.isClearPageOn &&  !GameManager.instance.isSkillSelectPageOn)
            {
                GameManager.instance.isSettingPageOn = true;
                GameManager.instance.SettingPageObject.SetActive(true);
            }
            // 레벨업
            else if (GameManager.instance.isSkillSelectPageOn && !GameManager.instance.isDeadPageOn && !GameManager.instance.isClearPageOn && !GameManager.instance.isPausePageOn || GameManager.instance.isSkillSelectPageOn)
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
            }
            else if (GameManager.instance.isPausePageOn && GameManager.instance.isSettingPageOn) // Pause 화면
            {
                GameManager.instance.isSettingPageOn = false;
            }
            else if (GameManager.instance.isClearPageOn && GameManager.instance.isSettingPageOn) // Clear 화면
            {
                GameManager.instance.isSettingPageOn = false;
            }
            else if (GameManager.instance.isDeadPageOn && GameManager.instance.isSettingPageOn) // 게임 오버
            {
                GameManager.instance.isSettingPageOn = false;
            }
            else if (GameManager.instance.isSkillSelectPageOn) // Skill 선택화면
            {
                GameManager.instance.isSettingPageOn = false;
            }

            GameManager.instance.SettingPageObject.SetActive(false);
        }
    }

    private void OptionBackButtonClicked()
    {
        // 인게임 중 Setting Page 켜져있을 떄
        if (!GameManager.instance.isPausePageOn && !GameManager.instance.isClearPageOn && !GameManager.instance.isSkillSelectPageOn && !GameManager.instance.isDeadPageOn)
        {
            Time.timeScale = 1; // 화면 풀기
            PauseButtonObject.interactable = true;
        }
        // 기존 화면정지(Pause, SkillSelect, Clear, Dead) 돼있을 때 Setting Page 켜져있을 때
        // Pause 화면, SkillSelect
        else if (GameManager.instance.isPausePageOn && !GameManager.instance.isSkillSelectPageOn && !GameManager.instance.isClearPageOn  && !GameManager.instance.isDeadPageOn)
        {
            PauseButtonObject.interactable = true;
        }
        // SkillSelect
        else if ((GameManager.instance.isSkillSelectPageOn && !GameManager.instance.isPausePageOn && !GameManager.instance.isClearPageOn  && !GameManager.instance.isDeadPageOn))
        {
            PauseButtonObject.interactable = false;
        }
        // Clear, Dead
        else if (GameManager.instance.isClearPageOn || GameManager.instance.isDeadPageOn && !GameManager.instance.isPausePageOn && !GameManager.instance.isSkillSelectPageOn)
        {
            PauseButtonObject.interactable = false;
        }

        GameManager.instance.isSettingPageOn = false;
        GameManager.instance.SettingPageObject.SetActive(false);
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