using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    private StatisticsManager statsManager;
    [SerializeField] private Button buttonResume;
    [SerializeField] private Button buttonNextLevel;
    [SerializeField] private Button buttonMainMenu;
    [SerializeField] private GameObject parentGo;
    private AudioManager audioManager;
    
    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
        statsManager = GameManager.Instance.GetComponent<StatisticsManager>();
    }

    public void Pause()
    {
        audioManager.Play("Game_Pause");
        audioManager.Play("Game_PauseLoop");
        Time.timeScale = 0;
        parentGo.SetActive(true);
        buttonResume.Select();
    }

    public void Resume()
    {
        audioManager.Play("Game_Resume");
        audioManager.Stop("Game_PauseLoop");
        Time.timeScale = 1;
        parentGo.SetActive(false);
        
        if(statsManager.CanGoToNextLevel)
            buttonNextLevel.Select();
        else
            buttonMainMenu.Select();
    }
    
    public void Restart()
    {
        audioManager.Stop("Game_PauseLoop");
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
