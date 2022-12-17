using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHeadHUD : MonoBehaviour {

    private List<PlayerManager> players;
    [SerializeField] private bool showHeadHUD;
    [SerializeField] private GameObject headHUDPrefab;
    private List<GameObject> playersUI;

    private void Start() {
        players = FindObjectsOfType<PlayerManager>().ToList();
        playersUI = new List<GameObject>();
    }

    private void Update() {

        if (players.Count == 0) {
            players = FindObjectsOfType<PlayerManager>().ToList();

            if (transform.childCount == 0) {
                for (int i = 0; i < players.Count; i++) {
                    GameObject headHUD = Instantiate(headHUDPrefab,Vector3.zero,Quaternion.identity);
                    headHUD.transform.parent = transform;
                    playersUI.Add(headHUD);
                }
            }
            
        }

        if (showHeadHUD) {
            foreach (PlayerManager player in players) {
                List<PlayerConfiguration> configs = ApplicationManager.Instance.GetPlayerConfigs();
                PlayerConfiguration config = configs.Where(c => c.PlayerSprite == player.sprite.sprite).ToList()[0];
                
                Vector2 position = Camera.main.WorldToScreenPoint(player.transform.position);
                position.y += 85;

                playersUI[config.PlayerIndex].transform.position = position;
                playersUI[config.PlayerIndex].transform.GetChild(0).gameObject.GetComponent<Image>().sprite = config.PlayerArrowSpriteHUD;
                playersUI[config.PlayerIndex].transform.GetChild(0).GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = "P" + (config.PlayerIndex + 1);
            }
        }
    }
}
