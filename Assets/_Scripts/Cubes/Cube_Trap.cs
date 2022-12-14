using Data;
using UnityEngine;

public class Cube_Trap : Cube
{
    
    [SerializeField] private float _sideKnockForce;
    [SerializeField] private int _damageAmount;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player") || ApplicationManager.Instance.GameState != GAME_STATE.PLAYING) return;

        var playerSystemManager = collision.gameObject.GetComponent<PlayerManager>();
        var vec = collision.GetContact(0).normal;

        if (vec == new Vector2(0, -1) || vec == new Vector2(0, 1))
        {
            Debug.Log("Not side");
            var rand = UnityEngine.Random.Range(0, 2);
            KnockBack(rand == 0 ? new Vector2(0.5f, -vec.y) : new Vector2(-0.5f, -vec.y), playerSystemManager);
            return;
        }
        
        KnockBack(new Vector2(-vec.x, -vec.y), playerSystemManager);

    }

    void KnockbackRandom(PlayerManager playerSystem, Vector3 sideDirection)
    {
        var rand = UnityEngine.Random.Range(0, 2);

        if (rand == 0)
        {
            KnockBack((Vector3.left / _sideKnockForce + sideDirection).normalized, playerSystem);
            Debug.DrawLine(transform.position, transform.position + (Vector3.left / _sideKnockForce + sideDirection).normalized);
        }
            
        else
        {
            KnockBack((Vector3.right / _sideKnockForce + sideDirection).normalized, playerSystem);
            Debug.DrawLine(transform.position, transform.position + (Vector3.right / _sideKnockForce + sideDirection).normalized);
        }

    }

    private void KnockBack(Vector3 vec, PlayerManager playerSystem)
    {
        playerSystem.OnDamage(this, _damageAmount, vec * 10);
    }
}