using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallaxe: MonoBehaviour
{
    public float multiplier;
    public GameObject camera;
    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
       

    }
    private void Awake()
    {
        startPosition = transform.position;
        Debug.Log(gameObject.name + startPosition);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (startPosition == null) return;

        transform.position = new Vector3(startPosition.x + (multiplier * camera.transform.position.x), transform.position.y, transform.position.z);
    }


}
