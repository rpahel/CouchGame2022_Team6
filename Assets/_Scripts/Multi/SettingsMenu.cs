using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    private Resolution[] resolutions;
    [SerializeField] private Button buttonPlay;
    [SerializeField] private Button buttonSkip;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject tutorielMenu;
    [SerializeField] private EventSystem eventSystem;

    private AudioManager audioManager;

    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        var options = new List<string>();

        int currentResolutionindex = 0;
        for (int x = 0; x < resolutions.Length; x++)
        {
            string option = resolutions[x].width + " x " + resolutions[x].height;
            options.Add(option);

            if (resolutions[x].width == Screen.currentResolution.width && 
                resolutions[x].height == Screen.currentResolution.height)
            {
                currentResolutionindex = x;
            }
        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionindex;
        resolutionDropdown.RefreshShownValue();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void SetVolumeMusic(float volume)
    {
        audioMixer.SetFloat("volumeMusic", volume);
    }
    
    public void SetVolumeSfx(float volume)
    {
        audioMixer.SetFloat("volumeSFX", volume);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void ShowOptionsMenu()
    {
        optionsMenu.SetActive(true);
        mainMenu.SetActive(false);
        resolutionDropdown.Select();
    }
    
    public void ShowMainMenu()
    {
        audioManager.Play("Menu_Cancel");
        optionsMenu.SetActive(false);
        mainMenu.SetActive(true);
        buttonPlay.Select();
    }

    public void ShowTutorielMenu()
    {
        mainMenu.SetActive(false);
        tutorielMenu.SetActive(true);
        buttonSkip.Select();
    }
    
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
