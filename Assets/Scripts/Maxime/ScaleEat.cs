using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleEat : MonoBehaviour
{
    public float NbEaten = 1f;
    public Vector3 scaler = new Vector3(1,1,1);
   // public bool squashed;



    // Update is called once per frame
    void Update()
    {
        transform.localScale = scaler;

       
            scaler.y = Mathf.Lerp(scaler.y , NbEaten, .03f);
            scaler.x = Mathf.Lerp(scaler.x, NbEaten, .03f);
            scaler.z = Mathf.Lerp(scaler.z, NbEaten, .03f);

        

     
    }
}
