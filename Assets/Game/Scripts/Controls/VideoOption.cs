using System.Collections.Generic;
using Google.Protobuf.WellKnownTypes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoOption : MonoBehaviour
{
    List<Resolution> resolutions = new List<Resolution>();
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullScreenToggleButton;

    FullScreenMode screenMode;
    public int resolutionNum; // 해상도 선택가능한 목록

    void Start()
    {
        InitUI();
    }

    void InitUI()
    {
        for(int i = 0; i<Screen.resolutions.Length; i++)
        {
                resolutions.Add(Screen.resolutions[i]);
        }

        resolutionDropdown.options.Clear();

        int optionNum = 0;
        foreach (Resolution item in resolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            //option.text = item.width + "x" + item.height + " " + item.refreshRate + "hz";
            option.text = item.width + "x" + item.height;
            resolutionDropdown.options.Add(option);

            if (item.width == Screen.width && item.height == Screen.height)
            {
                resolutionDropdown.value = optionNum;
            }
            optionNum++;
        }

        resolutionDropdown.RefreshShownValue();

        fullScreenToggleButton.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }

    // 선택한 해상도로 변경하는 함수
    public void DropOptionChange(int x)
    {
        resolutionNum = x;
    }

    public void FullScreenButton(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
    }

    // Save 버튼 클릭 시 저장하는 함수 .. Save 클릭 시 해상도랑 전체화면 바뀌는지 체크해야함 !
    public void SaveButtonClick()
    {
        Screen.SetResolution(resolutions[resolutionNum].width,
            resolutions[resolutionNum].height,
        screenMode
            );

        //Debug.Log(resolutions[resolutionNum].width + ", " +  resolutions[resolutionNum].height);
    }
}
