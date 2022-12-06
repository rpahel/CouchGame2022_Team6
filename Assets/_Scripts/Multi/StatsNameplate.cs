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
    
    
    public void SetStats(byte index, float damage, int kills, int deaths)
    {
        var playerConfigs = ApplicationManager.Instance.GetPlayerConfigs().ToArray();
        manager = ApplicationManager.Instance;
        
        switch (index)
        {
            case 0:
                textRank.text = "1";
                imagePlayer.sprite = playerConfigs[0].PlayerSprite;
                facePlayer.sprite = playerConfigs[0].PlayerFaceSprite;
                break;
            case 1:
                textRank.text = "2";
                imagePlayer.sprite = playerConfigs[1].PlayerSprite;
                facePlayer.sprite = playerConfigs[1].PlayerFaceSprite;
                break;
            case 2:
                textRank.text = "3";
                imagePlayer.sprite = playerConfigs[2].PlayerSprite;
                facePlayer.sprite = playerConfigs[2].PlayerFaceSprite;
                break;
            case 3:
                textRank.text = "4";
                imagePlayer.sprite = playerConfigs[3].PlayerSprite;
                facePlayer.sprite = playerConfigs[3].PlayerFaceSprite;
                break;
        }

        textDamage.text = damage.ToString();
        textKills.text = kills.ToString();
        textDeaths.text = deaths.ToString();
    }
}
