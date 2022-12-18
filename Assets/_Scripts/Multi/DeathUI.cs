using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DeathUI : CoroutineSystem {

    [SerializeField] private GameObject deathUIPrefab;
    private List<PlayerManager> players;
    public static DeathUI Instance;

    private void Awake() => Instance = this;
    

    void Start() => players = new List<PlayerManager>();
    

    void Update() {
        if (players.Count == 0) 
            players = FindObjectsOfType<PlayerManager>().ToList();
    }

    public void CreateDeathUI(PlayerConfiguration killer, PlayerConfiguration death) {
        
        GameObject deathUI = Instantiate(deathUIPrefab);
        deathUI.transform.parent = transform;

        deathUI.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = killer.BackgroundKill;
        deathUI.transform.GetChild(1).gameObject.GetComponent<Image>().sprite = killer.PlayerIcon;
        deathUI.transform.GetChild(2).gameObject.GetComponent<Image>().sprite = killer.PlayerArrowSprite;
        deathUI.transform.GetChild(3).gameObject.GetComponent<Image>().sprite = death.PlayerIcon;
        
        
        RunDelayed(5f, () => {
            Destroy(deathUI);
        });
        

    }
}
