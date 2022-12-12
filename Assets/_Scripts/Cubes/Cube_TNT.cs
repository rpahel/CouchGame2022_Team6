using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.VisualScripting;
using UnityEngine;

public class Cube_TNT : CubeDestroyable {
    
    public TNT pattern;
    [Range(0, 100)] 
    public int damageEat;

    [SerializeField] private int timeBeforeExplosion;
    [SerializeField] private float timeBeforeAnimDissapear;
    [SerializeField] private SpriteRenderer spriteBomb;
    
    [HideInInspector] public bool startExplode;
    [SerializeField] private List<MeshRenderer> listMeshRenderer = new List<MeshRenderer>();
    [SerializeField] private List<MeshRenderer> listMeshRendererExplode = new List<MeshRenderer>();
    private CinemachineImpulseSource shakeSource;

    private void Awake()
    {
        foreach (MeshRenderer mesh in listMeshRenderer)
        {
            mesh.sortingOrder = 200;
        }
        
        shakeSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Explode(Transform colParent,PlayerManager source) => StartCoroutine(OnExplosion(colParent,source));
    
    private IEnumerator OnExplosion(Transform colParent,PlayerManager source) {
        startExplode = true;
        StartCoroutine(IAnimTrigger());
        yield return new WaitForSeconds(timeBeforeExplosion);
        shakeSource.GenerateImpulse(2f);
        GameManager.Instance.AudioManager.Play("TNT_Explode");
        StartCoroutine(IAnimFlame());
        foreach (Vector2 dir in pattern.pattern) {
            if (dir != Vector2.zero) {
                Vector3 direction = new Vector3(dir.x,dir.y,colParent.position.z);
                foreach (CubeDestroyable c in FindCubeInDirection(direction, colParent.GetChild(0).gameObject)) {

                    levelGenerator.AddToRespawnList(c);

                    Collider2D[] results = Physics2D.OverlapBoxAll(c.transform.position, new Vector2(levelGenerator.Scale - 0.15f, levelGenerator.Scale - 0.15f), 90);

                    foreach (Collider2D col in results) {
                        if (col.gameObject.TryGetComponent<PlayerManager>(out PlayerManager playerManager))
                            playerManager.OnDamage(source, damageEat, Vector3.zero);
                    }

                    if (c.gameObject != this.gameObject && c is Cube_TNT)
                        ((Cube_TNT)c).Explode(c.transform, source);
                    else
                        StartCoroutine(c.Suck(c.transform.GetChild(0).gameObject, colParent));
                    
                }
            }
        }

        startExplode = false;
        levelGenerator.AddToRespawnList(colParent.gameObject.GetComponent<CubeDestroyable>());
    }
    

    private List<CubeDestroyable> FindCubeInDirection(Vector3 direction,GameObject origin) {
        List<CubeDestroyable> cubesInDir = new List<CubeDestroyable>();
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin.transform.position,direction);
        
        foreach (RaycastHit2D hit in hits) {
            if (hit.collider != null &&
                hit.collider.gameObject.transform.parent.TryGetComponent<CubeDestroyable>(out CubeDestroyable cubeDestroyable)) {
                cubesInDir.Add(cubeDestroyable);
            }
        }

        cubesInDir.Add(origin.GetComponentInParent<CubeDestroyable>());

        return cubesInDir;
    }


    private IEnumerator IAnimFlame() {
        foreach (MeshRenderer mesh in listMeshRendererExplode)
        {
           // Debug.Log("mesh " + mesh);
            mesh.sortingOrder = 200;
            mesh.gameObject.SetActive(true);
        }

        Debug.Log("timeBeforeAnimDisappear "+ timeBeforeAnimDissapear);
        yield return new WaitForSeconds(timeBeforeAnimDissapear);
            
        foreach (MeshRenderer mesh in listMeshRendererExplode)
        {
            mesh.gameObject.SetActive(false);
        }
    }
    
    private IEnumerator IAnimTrigger()
    {
        float time = timeBeforeExplosion;
        GameManager.Instance.AudioManager.Play("TNT_Trigger");
        spriteBomb.color = Color.red;
        yield return new WaitForSeconds(time / 5);
        spriteBomb.color = Color.white;
        yield return new WaitForSeconds(time / 5);
        spriteBomb.color = Color.red;
        yield return new WaitForSeconds(time / 5);
        spriteBomb.color = Color.white;
        yield return new WaitForSeconds(time / 5);
        spriteBomb.color = Color.red;
        yield return new WaitForSeconds(time / 5);
        spriteBomb.color = Color.white;
    }

}
