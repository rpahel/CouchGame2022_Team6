using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public void DestroyObj(float time)
    {
        Destroy(this.gameObject, time);
    }
    
    

}
