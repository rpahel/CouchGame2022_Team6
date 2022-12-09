using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsNameplate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textRank;
    [SerializeField] private Image imagePlayer;
    [SerializeField] private Image facePlayer;
    [SerializeField] private TextMeshProUGUI textDamage;
    [SerializeField] private TextMeshProUGUI textKills;
    [SerializeField] private TextMeshProUGUI textDeaths;

    private ApplicationManager manager;
    
    
    public void SetStats(byte index, byte indexSprite, float damage, int kills, int deaths)
    {
        var playerConfigs = ApplicationManager.Instance.GetPlayerConfigs().ToArray();
        manager = ApplicationManager.Instance;
        
        switch (index)
        {
            case 0:
                textRank.text = "1";
                imagePlayer.sprite = playerConfigs[indexSprite].PlayerSprite;
                facePlayer.sprite = playerConfigs[indexSprite].PlayerFaceSprite;
                break;
            case 1:
                textRank.text = "2";
                imagePlayer.sprite = playerConfigs[indexSprite].PlayerSprite;
                facePlayer.sprite = playerConfigs[indexSprite].PlayerFaceSprite;
                break;
            case 2:
                textRank.text = "3";
                imagePlayer.sprite = playerConfigs[indexSprite].PlayerSprite;
                facePlayer.sprite = playerConfigs[indexSprite].PlayerFaceSprite;
                break;
            case 3:
                textRank.text = "4";
                imagePlayer.sprite = playerConfigs[indexSprite].PlayerSprite;
                facePlayer.sprite = playerConfigs[indexSprite].PlayerFaceSprite;
                break;
        }

        textDamage.text = damage.ToString();
        textKills.text = kills.ToString();
        textDeaths.text = deaths.ToString();
    }
}
