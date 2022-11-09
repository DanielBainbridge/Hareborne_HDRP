// Author Kim Talbot with noted ammendments by Daniel Bainbridge
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public AudioMixer audioMixer;
    Resolution[] resolutions;
    private Toggle fullscreenToggle;

    private bool isSetYet = false;

    public TMPro.TMP_Dropdown resolutionDropdown;


    void Start()
    {
        resolutions = Screen.resolutions;

        // clear out all the options
        resolutionDropdown.ClearOptions();
        // creating a list of strings which will be the options
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;

        // loop through every element in our resolutions array
        for (int i = 0; i < resolutions.Length; i++)
        {
            // for each of them we create a formatted string that displays the resolution
            string option = resolutions[i].width + "X" + resolutions[i].height;
            // we then add it to our options list
            options.Add(option);
            // here we are comparing the resolution of our width and then height and if they both match up we are looking at the correct resolution. 
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height)
            {
                // store the index of the correct resolution
                currentResolutionIndex = i;
            }
        }
        // once done looping through we add our options list to our resolution drop down. 
        resolutionDropdown.AddOptions(options);
        // set the dropdown to our current resolution
        resolutionDropdown.value = currentResolutionIndex;
        // refresh to actually display the correct resolution
        resolutionDropdown.RefreshShownValue();
    }
    // a method that sets the resolution of our screen
    public void SetResolution(int resolutionIndex)
    {
        if (!isSetYet)
        {
            isSetYet = true;
        }
        else
        {
            Resolution resolution = resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }
    }

    // This is a function that we use to set the MAIN mixer using the slider provided in game via the UI. 
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    // This is a function that we use to change the quality of resolution in game using the drop down UI. 
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        isFullscreen = fullscreenToggle.isOn;
        if (isFullscreen == true)
        {
            Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
        }
        else Screen.fullScreenMode = FullScreenMode.Windowed;
    }

    //Ammendment By Daniel Bainbridge
    public void SetCameraSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("CamSpeed", sensitivity);
    }
}
