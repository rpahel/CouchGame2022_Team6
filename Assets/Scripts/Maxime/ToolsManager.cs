using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Data;

public class ToolsManager : MonoBehaviour
{
    private Movement _movement;
    private PlayerManager_JULIEN _playerManager;
    private ShootProjectile _shootProjectile;
    private Eat _eat;
    private PlayerControls _controls;

    public GameObject canvaGang;
    private void Awake()
    {
       
        _playerManager = gameObject.GetComponent<PlayerManager_JULIEN>();
        _movement = GetComponent<Movement>();
        _shootProjectile = gameObject.GetComponent<ShootProjectile>();
        _eat = GetComponent<Eat>();
        _controls = new PlayerControls();
    }
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activeMenuCheat()
    {
        if (canvaGang.activeSelf == false)
        {
            canvaGang.SetActive(true);
        }

        else if (canvaGang.activeSelf == true)
        {
            canvaGang.SetActive(false);
        }
    }
}
