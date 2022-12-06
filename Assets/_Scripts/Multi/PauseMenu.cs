using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private Button buttonResume;
    [SerializeField] private Button buttonRestart;
    [SerializeField] private GameObject parentGo;
    private AudioManager audioManager;
    
    private void Awake()
    {
        audioManager = FindObjectOfType<AudioManager>();
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
        buttonRestart.Select();
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
