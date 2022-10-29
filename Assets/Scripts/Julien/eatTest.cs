using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eatTest : MonoBehaviour
{
    [Header("Script")] 
    [SerializeField] private EatScript eatScript;
    
    private void OnTriggerStay2D(Collider2D other)
    {
        eatScript.OnTriggerStayFunction(other);
    }
}
