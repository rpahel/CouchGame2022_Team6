using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartParallax : MonoBehaviour
{

    public void EnableParallax()
    {
        foreach(Transform child in transform)
        {
            if (child.TryGetComponent(out parallaxe parallax))
                parallax.StartEffect();
        }
    }
}
