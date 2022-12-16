using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsNameplate : MonoBehaviour
{
    
    [SerializeField] private Image menu;
    [SerializeField] public TextMeshProUGUI textRank;
    [SerializeField] private Image imagePlayer;
    [SerializeField] private Image facePlayer;
    [SerializeField] public TextMeshProUGUI textDamage;
    [SerializeField] private TextMeshProUGUI textKills;
    [SerializeField] private TextMeshProUGUI textDeaths;

    [SerializeField] private Color color;


    private byte _index;
    private byte _indexSprite;
    private float _damage;
    private int _kills;
    private int _deaths;

    private bool _isShow;

    private void Update() {
        
        if(_isShow)
            SetStats(_index,_indexSprite,_damage,_kills,_deaths);
    }

    public void SetStats(byte index, byte indexSprite, float damage, int kills, int deaths)
    {
        _index = index;
        _indexSprite = indexSprite;
        _damage = damage;
        _kills = kills;
        _deaths = deaths;

        _isShow = true;
        
        var playerConfigs = ApplicationManager.Instance.GetPlayerConfigs().ToArray();
        var config = playerConfigs[indexSprite];

        textRank.text =  (index + 1).ToString();
        imagePlayer.sprite = config.PlayerSprite;
        facePlayer.sprite = config.PlayerFaceSprite;
        menu.sprite = config.MenuSprite;

        textDamage.text = "SCORE \n <color=" + GameManager.Instance.GetColorForUI(config.PlayerColor) + "> " + damage;
        textKills.text = "KILLS \n <color=" + GameManager.Instance.GetColorForUI(config.PlayerColor) + "> "  + kills;
        textDeaths.text = "DEATHS \n <color=" + GameManager.Instance.GetColorForUI(config.PlayerColor) + "> "  +  deaths;
    }
}
