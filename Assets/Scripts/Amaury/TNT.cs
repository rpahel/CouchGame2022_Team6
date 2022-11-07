using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : MonoBehaviour {

    public Vector2[] pattern;

    private void Awake() {
        pattern = new Vector2[9];
        
        Debug.Log("pattern " + pattern.Length);
    }
}
