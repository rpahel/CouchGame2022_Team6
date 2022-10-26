using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eatTest1 : MonoBehaviour
{
    [SerializeField] private PlayerManager _playerManager;
    
    [Header("Eat")]
    public float reach = 1f;  
    [SerializeField, Range(0f, 1f)]
    private float filling = 0.12f;
    private bool canEat = true;
    [SerializeField, Range(0f, .5f)]
    private float eatCooldown = 0.5f;
    private Coroutine cooldownCoroutine;
    //[SerializeField] private int maxCubeMangeable = 6;
    //private int _cubeEated;
    private List<Collider2D> _listColliders = new List<Collider2D>();
    private int blockeated;

    private void EatCube(Cube_Edible cubeMangeable)
    {
        //canEat = false;
        //Debug.Log("eat");
        cubeMangeable.GetManged(this);
        /*_cubeEated;
        if (_cubeEated >= maxCubeMangeable)
        {
            canEat = false;
            StopAllCoroutines();
            cooldownCoroutine = StartCoroutine(CooldownCoroutine());
        }*/
        _playerManager.eatAmount += filling;
        _playerManager.eatAmount = Mathf.Clamp(_playerManager.eatAmount, 0f, 1f);
        //cooldownCoroutine = StartCoroutine(CooldownCoroutine());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.parent.CompareTag("CubeEdible"))
            _listColliders.Add(other);
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        _listColliders.Remove(other);
    }

    public void TryEat()
    {
        Debug.Log("try Eat");
        
        if (_playerManager.eatAmount >= 1f)
        {
            print("I'm full !!");
            return;
        }
        
        for (var index = 0; index < _listColliders.Count; index++)
        {
            var c = _listColliders[index];
            Cube_Edible cubeMangeable;
            if(c.gameObject.activeSelf == false) {continue;}
            if (c.transform.parent.TryGetComponent<Cube_Edible>(out cubeMangeable))
            {
                EatCube(cubeMangeable);
                blockeated++;
                //_listColliders.Remove(_listColliders[index]);
            }

        }
        Debug.Log(blockeated);
        blockeated = 0;
    }
    
    
}
