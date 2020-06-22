using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    [Header("Canvas")]
    [SerializeField] private GameObject mainMenu = null;
    [SerializeField] private GameObject settingsMenu = null;
    [SerializeField] private TMP_Dropdown resolutionDropdown = null;
    [SerializeField] private TMP_Dropdown qualityDropdown = null;
    private Resolution[] resolutions = null;
    
    [Header("Scenes")]
    [SerializeField] private string gameSceneName = "Game";
    [SerializeField] private string titleSceneName = "Title";
    private TransitionController transitionController = null;
    
    [Header("Audio")]
    [SerializeField] private AudioMixer master = null;

    public void Start()
    {
        this.SetResolutionDropdownOptions();
        this.SetQualityDropdownOptions();
        this.transitionController = TransitionController.GetInstance();
    }

    public void SetResolutionDropdownOptions()
    {
        int currrentResolutionIndex = 0;
        this.resolutions = Screen.resolutions;
        this.resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        for (int i = 0; i < this.resolutions.Length; i++)
        {
            if (this.resolutions[i].width == Screen.width && this.resolutions[i].height == Screen.height)
            {
                currrentResolutionIndex = i;
            }
            options.Add(this.resolutions[i].width + " x " + this.resolutions[i].height);
        }
        this.resolutionDropdown.AddOptions(options);
        this.resolutionDropdown.value = currrentResolutionIndex;
        this.resolutionDropdown.RefreshShownValue();
    }

    public void SetQualityDropdownOptions()
    {
        this.qualityDropdown.ClearOptions();
        this.qualityDropdown.AddOptions(QualitySettings.names.ToList());
        this.qualityDropdown.value = QualitySettings.GetQualityLevel();
        this.qualityDropdown.RefreshShownValue();
    }

    public void OpenSettings()
    {
        this.mainMenu.SetActive(false);
        this.settingsMenu.SetActive(true);
    }

    public void CloseSettings()
    {
        this.mainMenu.SetActive(true);
        this.settingsMenu.SetActive(false);
    }

    public void Play()
    {
        this.transitionController.Load(this.gameSceneName);
    }

    public void Quit()
    {
        Scene scene = SceneManager.GetActiveScene();
        if (scene.name != this.titleSceneName)
        {
            this.transitionController.Load(this.titleSceneName);
            return;
        }
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif   
    }

    public void SetMasterVolume(float volume)
    {
        this.master.SetFloat("MasterVolume", Mathf.Clamp(volume - 80f, -80f, 20f));
    }

    public void SetMusicVolume(float volume)
    {
        this.master.SetFloat("MusicVolume", Mathf.Clamp(volume - 80f, -80f, 20f));
    }

    public void SetSFXVolume(float volume)
    {
        this.master.SetFloat("SFXVolume", Mathf.Clamp(volume - 80f, -80f, 20f));
    }

    public void SetQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int index)
    {
        Resolution resolution = this.resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
