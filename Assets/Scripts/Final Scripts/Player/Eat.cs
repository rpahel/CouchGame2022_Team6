using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : MonoBehaviour
{
    private PlayerManager _playerManager;
    
    [Header("Eat")]
    public Transform pointeur;       
    public Transform pointeurBase;   
    public float reach = 1f;  
    [SerializeField, Range(0f, 1f)]
    private float filling = 0.12f;
    private bool canEat = true;
    [SerializeField, Range(0f, .5f)]
    private float eatCooldown = 0.5f;
    private Coroutine cooldownCoroutine;
    private float angle;   
    
    // Start is called before the first frame update
    void Awake()
    {
        _playerManager = gameObject.GetComponent<PlayerManager>();
        pointeur.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        pointeur.gameObject.SetActive(_playerManager.InputVector.sqrMagnitude > 0.1f ? true : false);
        if (pointeur.gameObject.activeSelf)
        {
            angle = Mathf.Atan2(_playerManager.InputVector.y, _playerManager.InputVector.x);
            pointeur.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
        }
    }

    private void FixedUpdate()
    {
        if(_playerManager.CanEat)
            TryEat();
    }

    private void EatCube(Cube_Edible cubeMangeable)
    {
        //CubeMangeable.GetManged(this);
        _playerManager.eatAmount += filling;
        _playerManager.eatAmount = Mathf.Clamp(_playerManager.eatAmount, 0f, 1f);
        canEat = false;
        cooldownCoroutine = StartCoroutine(CooldownCoroutine());
    }
    
    public void TryEat()
    {
        Debug.Log("try Eat");
        if (!canEat)
        {
            print("I'm on eating cooldown !");
            return;
        }

        if (_playerManager.eatAmount >= 1f)
        {
            print("I'm full !!");
            return;
        }
        
        RaycastHit2D hit = Physics2D.Raycast(pointeurBase.position, _playerManager.InputVector, reach);
        if (hit)
        {
            if (hit.transform.parent.CompareTag("CubeEdible"))
            {
                Cube_Edible cubeMangeable;
                if (hit.transform.parent && hit.transform.parent.TryGetComponent<Cube_Edible>(out cubeMangeable))
                {
                    EatCube(cubeMangeable);
                }
                else
                    print("Pas de Raf_CubeMangeable dans le cube vis�.");
            }
        }
    }
    
    IEnumerator CooldownCoroutine()
    {
        yield return new WaitForSeconds(eatCooldown);
        canEat = true;
        StopCoroutine(cooldownCoroutine);
    }
}
