using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePointeur : MonoBehaviour
{
    private PlayerManager _playerManager;
    
    [Header("Eat")]
    public Transform pointeur;       
    public Transform pointeurBase;
    private float angle;   
    
    // Start is called before the first frame update
    void Awake()
    {
        pointeur.gameObject.SetActive(false);
        _playerManager = gameObject.GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        pointeur.gameObject.SetActive(_playerManager.InputVector.sqrMagnitude > 0.1f ? true : false);
        if (!pointeur.gameObject.activeSelf) return;
        
        angle = Mathf.Atan2(_playerManager.InputVector.y, _playerManager.InputVector.x);
        pointeur.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
    }
}
