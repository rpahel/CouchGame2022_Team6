using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    int progress = 0;
    [SerializeField] private Slider _slider;
 

     public PlayerInputHandler playerHandler;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _slider.value = playerHandler._holdCooldown;
    }

    public void UpdateProgress()
    {
        
    }
}
