using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class VideoOption : MonoBehaviour
{
    FullScreenMode screenMode;
    
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown fullScreenDropdown;
    List<Resolution> resolutions = new List<Resolution>();
    public int resolutionNum;

    void Start() {
        InitUI();
    }

    private void InitUI() {
        for(int i = 0; i < Screen.resolutions.Length; i++) {
            if (Screen.resolutions[i].refreshRate == 60)
                resolutions.Add(Screen.resolutions[i]);
        }

        resolutionDropdown.options.Clear();

        int optionNum = 0;
        foreach(Resolution item in resolutions) {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData();
            option.text = item.width + " x " + item.height + " " + item.refreshRate + "hz";
            resolutionDropdown.options.Add(option);

            if(item.width == Screen.width && item.height == Screen.height) {
                resolutionDropdown.value = optionNum;
            }
            optionNum++;
        }
        resolutionDropdown.RefreshShownValue();

        if(Screen.fullScreenMode == FullScreenMode.Windowed) {
            fullScreenDropdown.value = 0;
            screenMode = FullScreenMode.Windowed;
        }
        else if(Screen.fullScreenMode == FullScreenMode.ExclusiveFullScreen) {
            fullScreenDropdown.value = 1;
            screenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else {
            fullScreenDropdown.value = 2;
            screenMode = FullScreenMode.FullScreenWindow;
        }
    }

    public void DropboxOptionChange(int x) {
        resolutionNum = x;
    }

    public void ScreenModeOptionChange(int x) {
        if(x == 0)
            screenMode = FullScreenMode.Windowed; // 창모드
        else if(x == 1)
            screenMode = FullScreenMode.ExclusiveFullScreen; // 전체화면
        else
            screenMode = FullScreenMode.FullScreenWindow; // 테두리 없는 창모드
    }
    public void OkBtn() {
        Screen.SetResolution(resolutions[resolutionNum].width, resolutions[resolutionNum].height, screenMode);
    }
}
