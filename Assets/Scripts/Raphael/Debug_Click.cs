using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debug_Click : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            OnClick();
        }    
    }

    void OnClick()
    {
        Vector3 clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward * 10f);
        Vector3 cameraPos = Camera.main.transform.position;
        RaycastHit2D hit = Physics2D.Raycast(cameraPos, (clickPos - cameraPos).normalized, Mathf.Infinity);
        if (hit && hit.collider.transform.parent && hit.collider.transform.parent.CompareTag("CubeEdible"))
        {
            hit.collider.GetComponentInParent<Cube_Edible>().OnExploded();
        }
    }
}
