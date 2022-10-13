using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using UnityEngine.PlayerLoop;

public class ScaleEat : MonoBehaviour
{
    private PlayerManager _playerManager;
    
    public SwitchSizeSkin switchSkin;
    
    public Vector3 scaler = new Vector3(1,1,1);
    public Mesh meshLittle;
    public Mesh meshAverage;
    public Mesh meshBig;
    public Mesh currentMesh;
    private MeshFilter _meshFilterGo;

    private void InitializedSize()
    {
        switchSkin = SwitchSizeSkin.Little;
    }
    private void Awake()
    {
        _playerManager = gameObject.GetComponent<PlayerManager>();
        _meshFilterGo = this.transform.GetChild(0).GetComponent<MeshFilter>();
        currentMesh = _meshFilterGo.mesh;

    }
    void Update()
    {
       scaleEat();
    }

    void scaleEat()
    {
        transform.localScale = scaler;
        _playerManager.eatAmount = Mathf.Clamp(_playerManager.eatAmount, 1f, 2.857143f);
        _playerManager.TextUI.text = (_playerManager.eatAmount * 100).ToString() + "%";

        //juste pour sa soit smooth
        scaler.y = Mathf.Lerp(scaler.y, _playerManager.eatAmount, .03f); 
        scaler.x = Mathf.Lerp(scaler.x, _playerManager.eatAmount, .03f);
        scaler.z = Mathf.Lerp(scaler.z, _playerManager.eatAmount, .03f);

        if (_playerManager.eatAmount >=  2.15f)
        {
            switchSkin = SwitchSizeSkin.Big;
            _meshFilterGo.mesh = meshBig;
            
        }
        else if (_playerManager.eatAmount <= 1.64f)
        {
            switchSkin = SwitchSizeSkin.Little;
            _meshFilterGo.mesh = meshLittle;
        }
        else
        {
            switchSkin = SwitchSizeSkin.Medium;
            _meshFilterGo.mesh = meshAverage;
        }
    }
}
