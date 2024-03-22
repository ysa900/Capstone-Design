using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoOptionManager : MonoBehaviour
{
    private LobbyManager lobbyManager;

    // 가능한 해상도 목록
    List<string> resolutionOptions = new List<string>() {
        "1280x800",
        "1366x768",
        "1152x864",
        "1280x1024",
        "1280x720"
    };

    public TMP_Dropdown resolutionDropdown;
    public Toggle fullScreenToggleButton;

    private string prevResolution; // 이전 해상도 설정을 저장할 변수
    private bool prevFullScreen; // 이전 전체 화면 모드 설정을 저장할 변수


    void Start()
    {
        InitUI();
    }

    private void Awake()
    {
        lobbyManager = FindAnyObjectByType<LobbyManager>();
        lobbyManager.resolutionSaveButtonObject.GetComponent<UnityEngine.UI.Button>();
        lobbyManager.resolutionSaveButtonObject.onClick.AddListener(resolutionSaveButtonClick);

        lobbyManager.SettingPageBackButtonObject.onClick.AddListener(RestoreSettings); // Back 버튼에 클릭 리스너 추가

        // 이전 설정 값 불러오기
        prevResolution = PlayerPrefs.GetString("PreviousResolution", "1280x720");
        prevFullScreen = PlayerPrefs.GetInt("PreviousFullScreen", 1) == 1 ? true : false;

        // 이전 설정으로 UI 초기화
        resolutionDropdown.value = resolutionOptions.IndexOf(prevResolution);
        resolutionDropdown.RefreshShownValue();
        fullScreenToggleButton.isOn = prevFullScreen;
    }

    void InitUI()
    {
        // 해상도 드롭다운 설정
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionOptions);

        // 디폴트 해상도 설정
        resolutionDropdown.value = resolutionOptions.IndexOf("1280x720");
        resolutionDropdown.RefreshShownValue();

        // Full Screen 토글 설정
        fullScreenToggleButton.isOn = Screen.fullScreen;
    }

    // Save 버튼 클릭 시 설정 적용
    public void resolutionSaveButtonClick()
    {
        // 선택한 해상도로 변경
        string selectedResolution = resolutionOptions[resolutionDropdown.value];
        string[] resolutionParts = selectedResolution.Split('x');
        int width = int.Parse(resolutionParts[0]);
        int height = int.Parse(resolutionParts[1]);
        Screen.SetResolution(width, height, fullScreenToggleButton.isOn);

        // 변경된 설정 저장
        PlayerPrefs.SetString("PreviousResolution", selectedResolution);
        PlayerPrefs.SetInt("PreviousFullScreen", fullScreenToggleButton.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    // Back 버튼 클릭 시 이전 설정으로 복원
    public void RestoreSettings()
    {
        // 이전 설정으로 해상도 및 전체 화면 모드 복원
        resolutionDropdown.value = resolutionOptions.IndexOf(prevResolution);
        fullScreenToggleButton.isOn = prevFullScreen;
    }
}