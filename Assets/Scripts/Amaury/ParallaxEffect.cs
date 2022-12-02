using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour {

    private Transform camera;
    private Vector3 startPos;

    private Material[] b_mats;
    private float[] b_speeds;
    private GameObject[] backgrounds;

    private float farthestBack;

    [Range(0.01f,0.05f)]
    public float parallaxSpeed;

    private float distance;
    
    
    void Start() {
        camera = Camera.main.transform;
        startPos = camera.position;
        
        int backCount = transform.childCount;
        b_mats = new Material[backCount];
        b_speeds = new float[backCount];
        backgrounds = new GameObject[backCount];

        for (int i = 0; i < backCount; i++) {
            backgrounds[i] = transform.GetChild(i).gameObject;
            b_mats[i] = backgrounds[i].GetComponent<Renderer>().material;
        }
        
        CalculateBackgroundsSpeed(backCount);
    }

    private void CalculateBackgroundsSpeed(int backCount) {

        for (int i = 0; i < backCount; i++) {
            if (backgrounds[i].transform.position.z - camera.position.z > farthestBack)
                farthestBack = backgrounds[i].transform.position.z - camera.position.z;
        }

        for (int i = 0; i < backCount; i++) 
            b_speeds[i] = 1 - (backgrounds[i].transform.position.z - camera.position.z) / farthestBack;
        
        
    }

    private void LateUpdate() {
        distance = camera.position.x - startPos.x;

        for (int i = 0; i < backgrounds.Length; i++) {
            float speed = b_speeds[i] * parallaxSpeed;
            b_mats[i].SetTextureOffset("_MainTex", new Vector2(distance,0) * speed);
        }
    }
}
