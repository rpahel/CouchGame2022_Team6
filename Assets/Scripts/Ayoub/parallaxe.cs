using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallaxe: MonoBehaviour
{
    public float multiplier;
    public GameObject camera;
    private Vector3 startPosition;
    
    public void StartEffect()
    {
        startPosition = transform.position;
        Debug.Log(gameObject.name + " " + startPosition);
    }

    void FixedUpdate()
    {
        if (startPosition != null)
        {
            transform.position = new Vector3(startPosition.x + (multiplier  * camera.transform.position.x),
                transform.position.y, transform.position.z);
            Debug.Log("camX " + camera.transform.position.x);
        }
    }


}
