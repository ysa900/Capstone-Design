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

        // GoToNextScene 버튼 눌렀을 때
        UnityEngine.UI.Button GoToNextSceneButton = GoToNextSceneButtonObject.GetComponent<UnityEngine.UI.Button>();
        GoToNextSceneButton.onClick.AddListener(goToNextSceneButtonClicked);

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

        SceneManager.LoadScene("Stage1"); // Stage1 으로 돌아가기(Stage1씬 Load) 
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
        //// Pause페이지 On
        //if(GameManager.instance.isPausePageOn && !GameManager.instance.isClearPageOn && !GameManager.instance.isSkillSelectPageOn && !GameManager.instance.isDeadPageOn)
        //{
        //    GameManager.instance.isSettingPageOn = true;
        //    GameManager.instance.SettingPageObject.SetActive(true);
        //    OptionButtonObject.interactable = false;
        //}

        //// 레벨업 후 스킬 선택할 때
        //if (!GameManager.instance.isPausePageOn && !GameManager.instance.isClearPageOn && GameManager.instance.isSkillSelectPageOn && !GameManager.instance.isDeadPageOn)
        //{
        //    GameManager.instance.isSettingPageOn = true;
        //    GameManager.instance.SettingPageObject.SetActive(true);
        //    OptionButtonObject.interactable = false;
        //}
        //// 클리어 화면 On
        //if (!GameManager.instance.isPausePageOn && !GameManager.instance.isClearPageOn && GameManager.instance.isSkillSelectPageOn && !GameManager.instance.isDeadPageOn)
        //{
        //    GameManager.instance.isSettingPageOn = true;
        //    GameManager.instance.SettingPageObject.SetActive(true);
        //    OptionButtonObject.interactable = false;
        //}

        //// 플레이어 죽은 화면 On
        //if (!GameManager.instance.isPausePageOn && !GameManager.instance.isClearPageOn && GameManager.instance.isSkillSelectPageOn && !GameManager.instance.isDeadPageOn)
        //{
        //    GameManager.instance.isSettingPageOn = true;
        //    GameManager.instance.SettingPageObject.SetActive(true);
        //    OptionButtonObject.interactable = false;
        //}

        // 인게임 중
        if (!GameManager.instance.isPausePageOn && !GameManager.instance.isClearPageOn && !GameManager.instance.isSkillSelectPageOn && !GameManager.instance.isDeadPageOn)
        {
            GameManager.instance.isSettingPageOn = true;
            Time.timeScale = 0; // 화면 멈추기
            GameManager.instance.SettingPageObject.SetActive(true);
            OptionButtonObject.interactable = false;
        }
        else // 나머지 상황: Pause, 레벨업, 클리어, 플레이어 죽으면
        {
            GameManager.instance.isSettingPageOn = true;
            GameManager.instance.SettingPageObject.SetActive(true);
            OptionButtonObject.interactable = false;
        }
        

        //// Pause, 레벨업, 클리어, 플레이어 죽으면
        //if (GameManager.instance.isPausePageOn || GameManager.instance.isClearPageOn || GameManager.instance.isSkillSelectPageOn || GameManager.instance.isDeadPageOn)
        //{
        //    GameManager.instance.isSettingPageOn = true;
        //    GameManager.instance.SettingPageObject.SetActive(true);
        //    OptionButtonObject.interactable = false;
        //}

        //GameManager.instance.isSettingPageOn = true;
        //GameManager.instance.SettingPageObject.SetActive(true);
        //OptionButtonObject.interactable = false;
    }

    private void SettingPageBackButtonClicked()
    {
        GameManager.instance.isSettingPageOn = false;
        OptionButtonObject.interactable = true;

        // 인게임 중
        if (!GameManager.instance.isPausePageOn && !GameManager.instance.isClearPageOn && !GameManager.instance.isSkillSelectPageOn && !GameManager.instance.isDeadPageOn)
        {
            Time.timeScale = 1; // 화면 풀기
        }

        GameManager.instance.SettingPageObject.SetActive(false);
    }

    private void goToNextSceneButtonClicked()
    {
        switch(SceneManager.GetActiveScene().name)
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