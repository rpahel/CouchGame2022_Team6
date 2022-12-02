using System;
using System.Collections;
using System.Collections.Generic;
//using Assets.SimpleLocalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupMenuController : MonoBehaviour
{
    private int PlayerIndex;

    //[SerializeField] private LocalizedText text;

    [SerializeField] private GameObject readyPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Button readyButton;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image playerFace;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Text titleText;
    private Image menuImage;

    private float ignoreInputTime = 0.1f;
    private bool inputEnabled;

    private ApplicationManager manager;

    private void Awake()
    {
        menuImage = GetComponent<Image>();
        manager = ApplicationManager.Instance;
    }

    public void SetPlayerIndex(int pi)
    {
        PlayerIndex = pi;
        //text.hardText = (pi + 1).ToString();
        //titleText.text = "Player" + (pi + 1).ToString();
        ignoreInputTime = Time.time + ignoreInputTime;
    }

    private void Update()
    {
        if (Time.time > ignoreInputTime)
        {
            inputEnabled = true;
        }
    }

    public void SetPlayerGFX(int index)
    {
        if (!inputEnabled) { return;}

        var playerGfx = index switch
        {
            0 => manager.ListPlayersGfx[0],
            1 => manager.ListPlayersGfx[1],
            2 => manager.ListPlayersGfx[2],
            3 => manager.ListPlayersGfx[3],
            _ => new PlayerGfxUI()
        };
        
        ApplicationManager.Instance.SetPlayerGfx(PlayerIndex, playerGfx);
        playerImage.sprite = playerGfx.player;
        playerFace.sprite = playerGfx.face;
        menuImage.sprite = playerGfx.menu;
        buttonImage.sprite = playerGfx.button;
        readyPanel.SetActive(true);
        readyButton.Select();
        menuPanel.SetActive(false);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled) { return;}
        ApplicationManager.Instance.ReadyPlayer(PlayerIndex);
        readyButton.gameObject.SetActive(false);
    }
}
