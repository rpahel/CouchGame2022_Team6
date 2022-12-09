using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceManager : MonoBehaviour
{
    private PlayerManager playerManager;
    
    [SerializeField] private float cooldownFace;
    
    [SerializeField] private Sprite faceNormal;
    [SerializeField] private Sprite faceEat;
    [SerializeField] private Sprite faceFull;
    [SerializeField] private Sprite faceJump;
    [SerializeField] private Sprite faceShoot;
    [SerializeField] private Sprite faceDash;
    [SerializeField] private Sprite faceDamage;

    public Sprite FaceNormal => faceNormal;
    public Sprite FaceEat => faceEat;
    public Sprite FaceFull => faceFull;
    public Sprite FaceJump => faceJump;
    public Sprite FaceShoot => faceShoot;
    public Sprite FaceDash => faceDash;
    public Sprite FaceDamage => faceDamage;
    public float CooldownFace => cooldownFace;
    
    
    
    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
    }

    public void SetFaceNormal(Sprite sprite)
    {
        faceNormal = sprite;
        SetFace(sprite);
    }
    
    public void SetFace(Sprite sprite)
    {
        playerManager.SetFace(sprite);
    }

    public void ResetFace()
    {
        playerManager.SetFace(playerManager.fullness == 100 ? faceFull : faceNormal);
    }
}
