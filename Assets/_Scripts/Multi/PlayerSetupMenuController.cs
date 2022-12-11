using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
//using Assets.SimpleLocalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlayerSetupMenuController : MonoBehaviour
{
    private int playerIndex;
    private AudioManager audioManager;

    //[SerializeField] private LocalizedText text;
    
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private Image skinImageButtonLeft;
    [SerializeField] private Image skinImageButtonRight;
    [SerializeField] private Button readyButton;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image playerFace;
    [SerializeField] private Image buttonReadyImage;
    [SerializeField] private Image buttonBackImage;
    [SerializeField] private Text titleText;
    [SerializeField] private Sprite menuBlack;
    [SerializeField] private EventSystem eventSystem;
    private Image menuImage;

    private float ignoreInputTime = 0.1f;
    private bool inputEnabled;
    private bool isOnReadyPanel;
    private bool skinIsSelected;
    private PlayerGfxUI playerGfx;

    private ColorPlayer colorChoose;
    private List<GameObject> listButtonColorGo = new List<GameObject>();
    private ApplicationManager manager;

    private void Awake()
    {
        menuImage = GetComponent<Image>();
        manager = ApplicationManager.Instance;
        manager.listSetupMenuControllers.Add(this);
        audioManager = FindObjectOfType<AudioManager>();
        SetupColors();
    }

    private void SetupColors()
    {
        var index = 0;
        switch (manager.ListColorRemaining.Count)
        {
            case 1:
                index = -15;
                break;
            case 2:
                index = -45;
                break;
            case 3:
                index = -75;
                break;
            case 4:
                index = -105;
                break;
            default:
                Debug.LogError("Error in ListColorRemaining.Count");
                return;
        }

        for (int i = manager.ListColorRemaining.Count; i > 0; i--)
        {
            var currentIndex = i - 1;
            var buttonGo = Instantiate(manager.ListColorRemaining[currentIndex].buttonPrefab, transform.position, Quaternion.identity);
            buttonGo.transform.parent = menuPanel.transform;
            buttonGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, index);
            index += 60;
            buttonGo.GetComponent<Button>().onClick.AddListener(() => { SetPlayerGFX(manager.ListColorRemaining[currentIndex]);});
            listButtonColorGo.Add(buttonGo);
            
            if (i == 1 && !isOnReadyPanel)
            {
                eventSystem.SetSelectedGameObject(null);
                eventSystem.SetSelectedGameObject(buttonGo);
            }
            
        }
    }

    public void SwapSkins()
    {
        if (skinIsSelected)
        {
            manager.SetPlayerSkin(playerIndex, playerGfx.player);
            playerImage.sprite = playerGfx.player;
            skinIsSelected = false;
        }
        else
        {
            manager.SetPlayerSkin(playerIndex, playerGfx.skin);
            playerImage.sprite = playerGfx.skin;
            skinIsSelected = true;
        }
    }

    public void RefreshColors()
    {
        foreach (GameObject go in listButtonColorGo)
        {
            Destroy(go);
        }
           
        listButtonColorGo.Clear();
        SetupColors();
    }

    public void SetPlayerIndex(int pi)
    {
        playerIndex = pi;
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

    public void SetPlayerGFX(ColorPlayer colorPlayer)
    {
        if (!inputEnabled) { return;}

        colorChoose = colorPlayer;
        playerGfx = colorChoose.index switch
        {
            0 => manager.ListPlayersGfx[0],
            1 => manager.ListPlayersGfx[1],
            2 => manager.ListPlayersGfx[2],
            3 => manager.ListPlayersGfx[3],
            _ => new PlayerGfxUI()
        };

        PlayRandomCharacterSound();
        ApplicationManager.Instance.SetPlayerGfx(playerIndex, playerGfx);
        playerImage.sprite = playerGfx.player;
        skinImageButtonLeft.sprite = playerGfx.buttonSkin;
        skinImageButtonRight.sprite = playerGfx.buttonSkin;
        playerFace.sprite = playerGfx.face;
        menuImage.sprite = playerGfx.menu;
        buttonReadyImage.sprite = playerGfx.button;
        buttonBackImage.sprite = playerGfx.button;
        readyPanel.SetActive(true);
        menuPanel.SetActive(false);
        isOnReadyPanel = true;
        manager.DeleteColor(colorChoose.colorName);
        TargetReadyButton();
    }

    private void TargetReadyButton()
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(readyButton.gameObject);
    }

    public void Back()
    {
        isOnReadyPanel = false;
        audioManager.Play("Menu_Cancel");
        readyPanel.SetActive(false);
        manager.BackOnColorSelector(colorChoose);
        menuImage.sprite = menuBlack;
        menuPanel.SetActive(true);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled) { return;}
        manager.ReadyPlayer(playerIndex);
        readyButton.gameObject.SetActive(false);
        buttonBackImage.gameObject.SetActive(false);
    }

    private void PlayRandomCharacterSound()
    {
        var rand = Random.Range(0, 4);

        switch (rand)
        {
            case 0:
                audioManager.Play("Menu_ChoosePlayer0");
                break;
            case 1:
                audioManager.Play("Menu_ChoosePlayer1");
                break;
            case 2:
                audioManager.Play("Menu_ChoosePlayer2");
                break;
            case 3:
                audioManager.Play("Menu_ChoosePlayer3");
                break;
        }
    }
}
