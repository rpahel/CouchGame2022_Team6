using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventUI : MonoBehaviour
{
    public void StartGame()
    {
        //Game state change
        GameManager.Instance.StartGame();
    }

    public void PlaySound(string soundName)
    {
        AudioManager.Instance.Play(soundName);
    }
}