using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
//using Assets.SimpleLocalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
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
    [SerializeField] private Image buttonReadyImage;
    [SerializeField] private Image buttonBackImage;
    [SerializeField] private Text titleText;
    [SerializeField] private Sprite menuBlack;
    [SerializeField] private EventSystem eventSystem;
    private Image menuImage;

    private float ignoreInputTime = 0.1f;
    private bool inputEnabled;

    private List<GameObject> listButtonColorGo = new List<GameObject>();
    private int indexColorTook;
    private ApplicationManager manager;

    private void Awake()
    {
        menuImage = GetComponent<Image>();
        manager = ApplicationManager.Instance;
        manager.listSetupMenuControllers.Add(this);
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
            Debug.Log(manager.ListColorRemaining[currentIndex].colorName + " " + manager.ListColorRemaining[currentIndex].index);
            buttonGo.GetComponent<Button>().onClick.AddListener(() => { SetPlayerGFX(manager.ListColorRemaining[currentIndex].index);});
            listButtonColorGo.Add(buttonGo);
            
            if (i == 1)
            {
                eventSystem.SetSelectedGameObject(null);
                eventSystem.SetSelectedGameObject(buttonGo);
            }
            
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

        indexColorTook = index;
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
        buttonReadyImage.sprite = playerGfx.button;
        buttonBackImage.sprite = playerGfx.button;
        readyPanel.SetActive(true);
        menuPanel.SetActive(false);
        
        manager.DeleteColor(index);
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(readyButton.gameObject);
    }

    public void Back()
    {
        readyPanel.SetActive(false);
        manager.BackOnColorSelector(indexColorTook);
        menuImage.sprite = menuBlack;
        menuPanel.SetActive(true);
    }

    public void ReadyPlayer()
    {
        if (!inputEnabled) { return;}
        ApplicationManager.Instance.ReadyPlayer(PlayerIndex);
        readyButton.gameObject.SetActive(false);
        buttonBackImage.gameObject.SetActive(false);
    }
}
