using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static InputManager;

public class InputManager : MonoBehaviour
{
    // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
    private static InputManager _instance;
    // 인스턴스에 접근하기 위한 프로퍼티
    public static InputManager instance
    {
        get
        {
            // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
            if (!_instance)
            {
                _instance = FindAnyObjectByType(typeof(InputManager)) as InputManager;

                if (_instance == null)
                    Debug.Log("no Singleton obj");
            }
            return _instance;
        }
    }

    // GameOver_GoTOLobby 버튼
    public static UnityEngine.UI.Button GVGoToLobbyButtonObject;

    // GameOver_Restart 버튼
    public static UnityEngine.UI.Button GVRestartButtonObject;

    // GameClear_GoTOLobby 버튼
    public static UnityEngine.UI.Button GCGoToLobbyButtonObject;

    // Pause 버튼
    public static UnityEngine.UI.Button PauseButtonObject;

    // Pause_GoTOLobby 버튼
    public static UnityEngine.UI.Button PGoToLobbyButtonObject;

    // Pause_Restart 버튼
    public static UnityEngine.UI.Button PRestartButtonObject;

    // Play 버튼
    public static UnityEngine.UI.Button PlayButtonObject;

    // Option 관련 버튼
    public static UnityEngine.UI.Button OptionButtonObject; // 플레이 화면의 Option 버튼
    // Option Back Button 관련 버튼
    public static UnityEngine.UI.Button OptionBackbuttonObject; // Setting 창 Back Button


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

        Init();
    }

    public void Init()
    {
        FindObjects();

        GVGoToLobbyButtonObject.interactable = true;
        GVRestartButtonObject.interactable = true;
        GCGoToLobbyButtonObject.interactable = true;
        PauseButtonObject.interactable = true;
        PGoToLobbyButtonObject.interactable = true;
        PRestartButtonObject.interactable = true;
        PlayButtonObject.interactable = true;
        OptionButtonObject.interactable = true;

        ButtonSetting();
    }

    void FindObjects()
    {
        Transform canvasTransform = GameObject.Find("Canvas").transform;

        GVGoToLobbyButtonObject = canvasTransform.Find("Game Over Menu/GoToLobby Button").GetComponent<Button>();
        GVRestartButtonObject = canvasTransform.Find("Game Over Menu/Restart Button").GetComponent<Button>();
        GCGoToLobbyButtonObject = canvasTransform.Find("Clear Menu/GoToLobby Button").GetComponent<Button>();
        PauseButtonObject = canvasTransform.Find("Pause Button").GetComponent<Button>();
        PGoToLobbyButtonObject = canvasTransform.Find("Pause Menu/GoToLobby Button").GetComponent<Button>();
        PRestartButtonObject = canvasTransform.Find("Pause Menu/Restart Button").GetComponent<Button>();
        PlayButtonObject = canvasTransform.Find("Pause Menu/Play Button").GetComponent<Button>();
        OptionButtonObject = canvasTransform.Find("Game Option Button").GetComponent<Button>();
        OptionBackbuttonObject = canvasTransform.Find("Setting Page/Back Button").GetComponent<Button>();
    }

    void ButtonSetting()
    {
        // 씬 내의 모든 버튼을 찾습니다.
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        // 각 버튼에 클릭 리스너를 추가합니다.
        foreach (Button button in allButtons)
        {
            button.onClick.AddListener(() => OnButtonClicked(button));
        }
    }

    public void DebugFunction()
    {


        /*Debug.Log("InputManager Canvas 객체들 검사");
        Debug.Log(GVGoToLobbyButtonObject);
        Debug.Log(GVRestartButtonObject);
        Debug.Log(GCGoToLobbyButtonObject);
        Debug.Log(PauseButtonObject);
        Debug.Log(PGoToLobbyButtonObject);
        Debug.Log(PRestartButtonObject);
        Debug.Log(PlayButtonObject);
        Debug.Log(OptionButtonObject);*/
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

        // SettingPage Back 버튼 눌렀을 때
        UnityEngine.UI.Button OptionBackbutton = OptionBackbuttonObject.GetComponent<UnityEngine.UI.Button>();
        OptionBackbuttonObject.onClick.AddListener(OptionBackButtonClicked);
    }

    // RestartButton이 눌렀을 때
    private void RestartButtonClicked()
    {
        GameAudioManager.instance.bgmPlayer.Stop(); // 현재 BGM 종료
        
        SceneManager.LoadScene("Stage1"); // Stage1 으로 돌아가기
    }

    // goToLobbyButton이 눌렀을 때
    private void goToLobbyButtonClicked()
    {
        GameAudioManager.instance.bgmPlayer.Stop(); // 현재 BGM 종료
        GameAudioManager.instance.soundData.isFirstLobby = true;
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
            if (!GameManager.SettingPageObject.activeSelf || !GameManager.instance.isGameOver && !GameManager.instance.isDeadPageOn && !GameManager.instance.isSkillSelectPageOn)
            {
                Time.timeScale = 0; // 인게임 플레이 중에만 멈추기
                PauseButtonObject.interactable = false;  // 인게임 플레이 중에만 Pause 버튼 비활성화
            }

            GameManager.instance.isSettingPageOn = true;
            GameManager.SettingPageObject.SetActive(true);
        }
        // Setting창 켜져있을 때 누른다면
        else
        {
            // Pause버튼이 비활성화돼있는 상황인 경우
            if (!GameManager.instance.isDeadPageOn && !GameManager.instance.isSkillSelectPageOn)
            {
                if (!GameManager.pauseObject.activeSelf || !GameManager.gameClearObject.activeSelf)
                    if (!GameManager.pauseObject.activeSelf)
                        Time.timeScale = 1;
                PauseButtonObject.interactable = true;  // 인게임 플레이 중에만 Pause 버튼 비활성화
            }

            GameManager.instance.isSettingPageOn = false;
            GameManager.SettingPageObject.SetActive(false);

        }
    }

    private void OptionBackButtonClicked()
    {
        if (GameManager.instance.isSettingPageOn)
        {
            // Pause버튼이 비활성화돼있는 상황인 경우
            if (!GameManager.instance.isDeadPageOn && !GameManager.instance.isSkillSelectPageOn)
            {
                if (!GameManager.pauseObject.activeSelf)
                    Time.timeScale = 1;
                PauseButtonObject.interactable = true;
            }

            GameManager.instance.isSettingPageOn = false;
            GameManager.SettingPageObject.SetActive(false);
        }

    }

    // 버튼이 클릭되었을 때 호출되는 메서드
    void OnButtonClicked(Button button)
    {
        GameAudioManager.instance.PlaySfx(GameAudioManager.Sfx.Select);
    }
}