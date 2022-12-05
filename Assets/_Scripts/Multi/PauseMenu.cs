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

    public void Pause()
    {
        Time.timeScale = 0;
        parentGo.SetActive(true);
        buttonResume.Select();
    }

    public void Resume()
    {
        Time.timeScale = 1;
        parentGo.SetActive(false);
        buttonRestart.Select();
    }
    
    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
