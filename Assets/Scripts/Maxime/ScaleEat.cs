using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using UnityEngine.PlayerLoop;

public class ScaleEat : MonoBehaviour
{
    private PlayerManager_JULIEN _playerManager;
    
    public Vector3 scaler = new Vector3(1,1,1);
    public Mesh meshLittle;
    public Mesh meshAverage;
    public Mesh meshBig;
    public Mesh currentMesh;
    private MeshFilter _meshFilterGo;
    private float maxScale = 2.857143f;
    [SerializeField] private float scaleSmooth;

    public List<Sprite> listSprite = new List<Sprite>();
    
    private Movement _movement;
    public float timeToLooseEat;
    private void InitializedSize()
    {
        _playerManager.SetSkin(SKIN_SIZE.Little);
    }
    private void Awake()
    {
        _movement = gameObject.GetComponent<Movement>();
        _playerManager = gameObject.GetComponent<PlayerManager_JULIEN>();
        _meshFilterGo = this.transform.GetChild(0).GetComponent<MeshFilter>();
        currentMesh = _meshFilterGo.mesh;
    }
    void Update()
    {
       scaleEat();
    }

    void scaleEat()
    {
        _playerManager.eatAmount = Mathf.Clamp(_playerManager.eatAmount, 0f, _playerManager.maxEatValue);
        _playerManager.textUI.text = (_playerManager.eatAmount * 100).ToString() + "%";

        //juste pour sa soit smooth
        float scaleTarget = Mathf.Lerp(1, maxScale, _playerManager.eatAmount); 
        //scaler.x = Mathf.Lerp(1,  maxScale, _playerManager.eatAmount);

        scaler.y = Mathf.Lerp(scaler.y, scaleTarget, scaleSmooth); 
        scaler.x = Mathf.Lerp(scaler.x,  scaleTarget, scaleSmooth);
        scaler.z = Mathf.Lerp(scaler.z, scaleTarget, scaleSmooth);
        transform.localScale = scaler;

        if (_playerManager.eatAmount >=  0.71f)
        {
            _playerManager.SetSkin(SKIN_SIZE.Big);
            _meshFilterGo.mesh = meshBig;
            _playerManager.imageUI.sprite = listSprite[2];
            _movement._canDash = true;
           // _playerManager.eatAmount -= Time.deltaTime * timeToLooseEat;
        }
        else if (_playerManager.eatAmount <= 0.35f)
        {
            _playerManager.SetSkin(SKIN_SIZE.Little);
            _meshFilterGo.mesh = meshLittle;
            _playerManager.imageUI.sprite = listSprite[1];
            _movement._canDash = false;
        }
        else
        {
            _playerManager.SetSkin(SKIN_SIZE.Medium);
            _meshFilterGo.mesh = meshAverage;
            _playerManager.imageUI.sprite = listSprite[0];
            _movement._canDash = false;
        }
    }
}
