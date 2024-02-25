using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VideoOption : MonoBehaviour
{
    List<Resolution> resolutions = new List<Resolution>();
    public TMP_Dropdown resolutionDropdown;

    void Start()
    {
        InitUI();
    }

    void InitUI()
    {
        resolutions.AddRange(Screen.resolutions);
        resolutionDropdown.options.Clear();

        foreach (Resolution item in resolutions)
        {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            //option.text = item.width + "x" + item.height + " " + item.refreshRateRatio + "hz";
            option.text = item.width + "x" + item.height;
            resolutionDropdown.options.Add(option);
        }

        resolutionDropdown.RefreshShownValue();
    }
}
