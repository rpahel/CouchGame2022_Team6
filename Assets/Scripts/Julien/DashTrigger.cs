using UnityEngine;
using Data;

public class DashTrigger : MonoBehaviour
{
    private ScaleEat _scaleEat;
    private PlayerManager _playerManager;
    private PlayerMovement _movement;

    [SerializeField] private float DamageBigEat = 0.4f;
    [SerializeField] private float DamageMediumEat = 0.4f;
    private void Awake()
    {
        _scaleEat = GetComponentInParent<ScaleEat>();
        _playerManager = GetComponentInParent<PlayerManager>();
        _movement = GetComponentInParent<PlayerMovement>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_playerManager.State != PlayerState.Dashing) return;
        
        if (other.GetComponent<Collider2D>().CompareTag("CubeEdible"))
        {
            Debug.Log("HitCubeEdible");
            other.gameObject.GetComponentInParent<Cube_Edible>().OnExploded();
        }
        else if (other.GetComponent<Collider2D>().CompareTag("Player") && _movement._canHit)
        {
            Debug.Log("HitPlayer");
            var pj = other.gameObject.GetComponent<PlayerManager>();
            switch (pj.SwitchSkin)
            {
                case SwitchSizeSkin.Big:
                    pj.eatAmount -= DamageBigEat;
                    break;
                case SwitchSizeSkin.Medium:
                    pj.eatAmount -= DamageMediumEat;
                    break;
                case SwitchSizeSkin.Little:
                    Debug.LogError("Dead");
                    pj.SetDead2();
                    break;
            }

            _movement._canHit = false;
        }
    }
}
