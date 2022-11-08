using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using UnityEngine.PlayerLoop;

public class ScaleEat : MonoBehaviour
{
    private PlayerManager _playerManager;
    private PlayerMovement _movement;
    
    public Vector3 scaler = new Vector3(1,1,1);
    public Mesh meshLittle;
    public Mesh meshAverage;
    public Mesh meshBig;
    public Mesh currentMesh;
    public bool canUpdateScale = true;
    private MeshFilter _meshFilterGo;
    
    [SerializeField] private float scaleSmooth;

    public List<Sprite> listSprite = new List<Sprite>();
    
    private void InitializedSize()
    {
        _playerManager.SetSkin(SwitchSizeSkin.Little);
    }
    private void Awake()
    {
        _movement = gameObject.GetComponent<PlayerMovement>();
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
        _playerManager.eatAmount = Mathf.Clamp(_playerManager.eatAmount, 0f, _playerManager.MaxEatValue);
        UpdateTextUI();

        if (!canUpdateScale) return;
        
        var scaleTarget = Mathf.Lerp(1, _playerManager.MaxScale, _playerManager.eatAmount);

        scaler.y = Mathf.Lerp(scaler.y, scaleTarget, scaleSmooth);
        scaler.x = Mathf.Lerp(scaler.x, scaleTarget, scaleSmooth);
        scaler.z = Mathf.Lerp(scaler.z, scaleTarget, scaleSmooth);
        //Scale factor
        transform.localScale = scaler;

        switch (_playerManager.eatAmount)
        {
            case >= 0.71f:
                _playerManager.SetSkin(SwitchSizeSkin.Big);
                _meshFilterGo.mesh = meshBig;
                _playerManager.imageUI.sprite = listSprite[2];
                _movement._canDash = true;
                break;
            case <= 0.35f:
                _playerManager.SetSkin(SwitchSizeSkin.Little);
                _meshFilterGo.mesh = meshLittle;
                _playerManager.imageUI.sprite = listSprite[1];
                _movement._canDash = false;
                break;
            default:
                _playerManager.SetSkin(SwitchSizeSkin.Medium);
                _meshFilterGo.mesh = meshAverage;
                _playerManager.imageUI.sprite = listSprite[0];
                _movement._canDash = false;
                break;
        }
    }

    private void UpdateTextUI()
    {
        var text = _playerManager.eatAmount * 100;

        switch (text)
        {
            case 100:
                _playerManager.textUI.text = text.ToString() + "%";
                break;
            case > 0 and < 10:
            case < 1:
                _playerManager.textUI.text = (text.ToString()).Substring(0, 1) + "%";
                break;
            default:
                _playerManager.textUI.text = (text.ToString()).Substring(0, 2) + "%";
                break;
        }
    }
}
