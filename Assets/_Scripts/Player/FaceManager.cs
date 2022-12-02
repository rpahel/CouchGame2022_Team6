using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceManager : MonoBehaviour
{
    private PlayerManager playerManager;
    
    [SerializeField] private Sprite faceNormal;
    [SerializeField] private Sprite faceEat;
    [SerializeField] private Sprite faceFull;
    [SerializeField] private Sprite faceJump;
    [SerializeField] private Sprite faceShoot;

    public Sprite FaceNormal => faceNormal;
    public Sprite FaceEat => faceEat;
    public Sprite FaceFull => faceFull;
    public Sprite FaceJump => faceJump;
    public Sprite FaceShoot => faceShoot;
    
    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    public void SetFaceNormal(Sprite sprite)
    {
        faceNormal = sprite;
    }

    public void SetFace(Sprite sprite)
    {
        playerManager.SetFace(sprite);
    }
}
