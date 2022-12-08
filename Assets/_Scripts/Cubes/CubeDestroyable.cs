using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CubeDestroyable : Cube {
    
    [SerializeField, Range(0.01f, .5f), Tooltip("Le temps que met le cube a parcourir la distance départ -> joueur")]
    private float moveDuration;
    [SerializeField, Range(-180, 180), Tooltip("Le degré de rotation du cube lors de l'animation quand tu le manges.")]
    private float rotationDegrees;
    
    public bool IsInAnimation { get; protected set; }
    [HideInInspector]
    public bool isOriginalCube;
    [SerializeField, Range(0.01f, 2f), Tooltip("Le temps que met le cube à apparaitre après être spawné par projectile.")]
    private float apparitionDuration;
    
    
    public IEnumerator Suck(GameObject cube, Transform player) {
        Vector3 cubeStartPos = cube.transform.position;
        Vector3 cubeStartRot = cube.transform.rotation.eulerAngles;
        Vector3 cubeEndRot = cubeStartRot + new Vector3(0, 0, rotationDegrees);

        Vector3 startScale = cube.transform.localScale;
        
        cube.transform.position -= Vector3.forward * 0.05f;
        float scaleFactor;
        float t = 0;

        while (t < 1f)
        {
            IsInAnimation = true;
            cube.transform.rotation = Quaternion.Euler(Vector3.Lerp(cubeStartRot, cubeEndRot, t));
            cube.transform.position = Vector3.Lerp(cubeStartPos, player.position, t);
            cube.transform.position = DOVirtual.EasedValue(cubeStartPos, player.position, t, Ease.InBack, 3f);
            scaleFactor = DOVirtual.EasedValue(startScale.x, 0, t, Ease.InBack, 2f);
            cube.transform.localScale = Vector3.one * scaleFactor;
            t += Time.deltaTime / moveDuration;
            yield return new WaitForFixedUpdate();
        }

        cube.transform.parent = this.transform;
        cube.transform.SetSiblingIndex(0);
        cube.transform.localPosition = Vector2.zero;
        
        if(cube.GetComponent<Collider2D>() != null)
            cube.GetComponent<Collider2D>().enabled = true;
        
        cube.SetActive(false);
        IsInAnimation = false;
    }
    
    /// <summary>
    /// Refais apparaitre ce cube sur la carte.
    /// </summary>

    public void GetBarfed(Vector2 impactPos, Color color) {
        cube.transform.localScale = Vector3.one;
        cube.transform.rotation = Quaternion.Euler(Vector3.zero);
        cube.transform.GetChild(0).GetComponent<SpriteRenderer>().color = color;
        cube.SetActive(true);
        isEaten = false;

        if(isOriginalCube)
            levelGenerator.RemoveFromRespawnList(this);

        StartCoroutine(BarfedAnimation(impactPos));
    }
    
    public void GetBarfed(Vector2 impactPos) {
        cube.transform.localScale = Vector3.one;
        cube.transform.rotation = Quaternion.Euler(Vector3.zero);
        cube.SetActive(true);
        cube.transform.parent.gameObject.SetActive(true);
        isEaten = false;

        if(isOriginalCube)
            levelGenerator.RemoveFromRespawnList(this);

        StartCoroutine(BarfedAnimation(impactPos));
    }

    IEnumerator BarfedAnimation(Vector2 impactPos) {
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one;
        Vector3 startPos = impactPos - (Vector2)transform.position;
        Vector3 endPos = Vector3.zero;
        float t = 0;

        while (t <= 1f) {
            IsInAnimation = true;
            cube.transform.localScale = DOVirtual.EasedValue(startScale, endScale, t, Ease.OutElastic, .5f);
            cube.transform.localPosition = DOVirtual.EasedValue(startPos, endPos, Mathf.InverseLerp(startScale.x, endScale.x, cube.transform.localScale.x), Ease.Linear);
            t += Time.fixedDeltaTime / apparitionDuration;
            yield return new WaitForFixedUpdate();
        }

        cube.transform.localScale = endScale;
        cube.transform.localPosition = endPos;

        IsInAnimation = false;
    }
}
