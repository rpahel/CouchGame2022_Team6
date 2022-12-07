using UnityEngine;

public class SpecialTriggerScript : MonoBehaviour
{
    [SerializeField] private PlayerStateSystem _playerSystem;
    private PlayerManager _playerManager;

    private void Start()
    {
        _playerManager = _playerSystem.PlayerManager;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Destructible"))
            collision.transform.parent.GetComponent<Cube_Edible>().GetEaten(collision.transform);

        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            collision.gameObject.GetComponent<PlayerManager>().OnDamage(
                damageDealer : _playerManager,
                damage : _playerManager.SpecialDamage,
                knockBackForce : _playerManager.SpecialInflictedKnockbackForce
                                 * (new Vector2(Mathf.Sign(_playerManager.Rb2D.velocity.x), 1)).normalized
                );

            if (_playerManager.specialStopsOnPlayerContact)
            {
                _playerSystem.SetState(new Knockback(_playerSystem));
                _playerSystem.SetKnockback(.5f * new Vector2(-_playerManager.Rb2D.velocity.x, 1f));
                gameObject.SetActive(false);
            }
        }
    }
}