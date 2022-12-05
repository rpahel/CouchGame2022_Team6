using System.Collections;
using System.Collections.Generic;
using Data;
using UnityEngine;

using DG.Tweening;

public class CubeDestroyable : Cube {
    
    [SerializeField, Range(0.01f, .5f), Tooltip("Le temps que met le cube a parcourir la distance départ -> joueur")]
    private float moveDuration;
    [SerializeField, Range(-180, 180), Tooltip("Le degré de rotation du cube lors de l'animation quand tu le manges.")]
    private float rotationDegrees;
    
    public bool IsInAnimation { get; protected set; }
    
    public IEnumerator Suck(GameObject cube, Transform player)
    {
        Vector3 cubeStartPos = cube.transform.position;
        Vector3 cubeStartRot = cube.transform.rotation.eulerAngles;
        Vector3 cubeEndRot = cubeStartRot + new Vector3(0, 0, rotationDegrees);
        cube.transform.position -= Vector3.forward * 0.05f;
        float scaleFactor;
        float t = 0;

        while (t < 1f)
        {
            IsInAnimation = true;
            cube.transform.rotation = Quaternion.Euler(Vector3.Lerp(cubeStartRot, cubeEndRot, t));
            cube.transform.position = Vector3.Lerp(cubeStartPos, player.position, t);
            cube.transform.position = DOVirtual.EasedValue(cubeStartPos, player.position, t, Ease.InBack, 3f);
            scaleFactor = DOVirtual.EasedValue(2.857143f, 0, t, Ease.InBack, 2f);
            cube.transform.localScale = Vector3.one * scaleFactor;
            t += Time.deltaTime / moveDuration;
            yield return new WaitForFixedUpdate();
        }

        cube.transform.parent = this.transform;
        cube.transform.SetSiblingIndex(0);
        cube.transform.localPosition = Vector2.zero;
        cube.GetComponent<Collider2D>().enabled = true;
        cube.SetActive(false);

        IsInAnimation = false;
    }
}
