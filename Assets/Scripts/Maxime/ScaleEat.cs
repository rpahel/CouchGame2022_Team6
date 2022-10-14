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
    private float maxScale = 2.857143f;

    public List<Sprite> listSprite = new List<Sprite>();
    
    private Movement _movement;
    public float timeToLooseEat;
    private void InitializedSize()
    {
        switchSkin = SwitchSizeSkin.Little;
    }
    private void Awake()
    {
        _movement = gameObject.GetComponent<Movement>();
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
        _playerManager.eatAmount = Mathf.Clamp(_playerManager.eatAmount, 0f, _playerManager.maxEatValue);
        _playerManager.textUI.text = (_playerManager.eatAmount * 100).ToString() + "%";

        //juste pour sa soit smooth
        scaler.y = Mathf.Lerp(1, maxScale, _playerManager.eatAmount); 
        scaler.x = Mathf.Lerp(1,  maxScale, _playerManager.eatAmount);
        scaler.z = Mathf.Lerp(1, maxScale, _playerManager.eatAmount);

        if (_playerManager.eatAmount >=  0.71f)
        {
            switchSkin = SwitchSizeSkin.Big;
            _meshFilterGo.mesh = meshBig;
            _playerManager.imageUI.sprite = listSprite[2];
            _movement._canDash = true;
           // _playerManager.eatAmount -= Time.deltaTime * timeToLooseEat;
        }
        else if (_playerManager.eatAmount <= 0.35f)
        {
            switchSkin = SwitchSizeSkin.Little;
            _meshFilterGo.mesh = meshLittle;
            _playerManager.imageUI.sprite = listSprite[1];
            _movement._canDash = false;
        }
        else
        {
            switchSkin = SwitchSizeSkin.Medium;
            _meshFilterGo.mesh = meshAverage;
            _playerManager.imageUI.sprite = listSprite[0];
            _movement._canDash = false;
        }
    }
}
