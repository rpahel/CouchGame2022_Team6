using CustomMaths;
using Data;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.LowLevel;

public class PlayerManager : MonoBehaviour
{
    #region Autres Scripts
    //============================
    private Rigidbody2D rb2D;
    private Collider2D coll;
    private PlayerMovement pMovement;
    private PlayerInputs pInputs;
    private PlayerShoot pShoot;
    private PlayerEat pEat;
    private PlayerSpecial pSpecial;

    // Getter
    public Rigidbody2D Rb2D => rb2D;
    public Collider2D PCollider => coll;
    public PlayerMovement PMovement => pMovement;
    public PlayerInputs PInputs => pInputs;
    public PlayerShoot PShoot => pShoot;
    public PlayerEat PEat => pEat;
    public PlayerSpecial PSpecial => pSpecial;
    #endregion

    #region Variables
    //============================
    [HideInInspector] public Color color;

    //============================
    private PLAYER_STATE playerState;
    public PLAYER_STATE PlayerState
    {
        get { return playerState; }
        set 
        {
            if (PlayerState != value)
            {
                PreviousPlayerState = playerState;
                playerState = value;
            }
        }
    }

    public PLAYER_STATE PreviousPlayerState { get; private set; }
    public Vector2 AimDirection { get; set; }
    public Vector2 LookDirection { get; set; }

    //============================
    [SerializeField] private float maxSize = 2.857f;
    [SerializeField] private float minSize = 1f;

    //============================ TODO : Supprimer
    public SpriteRenderer insideSprite;
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        TryGetAllComponents();
        SetSelfInComponents();
    }

    private void Update()
    {

        #if UNITY_EDITOR
        {
            Debug.DrawRay(transform.position - Vector3.forward, AimDirection * 5f, Color.cyan, Time.deltaTime);
        }
        #endif

        // On change LookDirection ici
        if (AimDirection.x > 0)
            LookDirection = Vector2.right;
        else if (AimDirection.x < 0)
            LookDirection = Vector2.left;
    }

    private void FixedUpdate()
    {
        UpdatePlayerScale();

        // On change la couleur du joueur en fonction de son état
        if (PlayerState == PLAYER_STATE.KNOCKBACKED)
            insideSprite.color = Color.red;
        else if (PlayerState == PLAYER_STATE.SHOOTING)
            insideSprite.color = Color.blue;
        else
            insideSprite.color = Color.white;
    }
    #endregion

    #region Custom_Functions

    /// <summary>
    /// Essaie de récupérer, dans le Player gameObject, le Component donné. Arrête le mode Play et retourne une erreur lorsque le Component n'est pas trouvé.
    /// </summary>
    /// <param name="component">Le component à récupérer.</param>
    /// <exception cref="Exception">Le component n'a pas été trouvé</exception>
    private void TryGetPlayerComponent<T>(out T component)
    {
        if (!TryGetComponent(out component))
        {

            #if UNITY_EDITOR
            {
                UnityEditor.EditorApplication.isPlaying = false;
            }
            #endif

            throw new Exception($"No {typeof(T)} component found in Player game object !");
        }
    }

    private void TryGetAllComponents()
    {
        TryGetPlayerComponent(out rb2D);
        TryGetPlayerComponent(out coll);
        TryGetPlayerComponent(out pMovement);
        TryGetPlayerComponent(out pEat);
        TryGetPlayerComponent(out pInputs);
        TryGetPlayerComponent(out pShoot);
        TryGetPlayerComponent(out pSpecial);
    }

    private void SetSelfInComponents()
    {
        pMovement.PManager = this;
        pEat.PManager = this;
        pInputs.PManager = this;
        pShoot.PManager = this;
        pSpecial.PManager = this;
    }

    /// <summary>
    /// Réduit la jauge de bouffe et knockback le joueur.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="damageDealer">L'objet responsable des dégats (un joueur, un piège, etc).</param>
    /// <param name="damage">La quantité de bouffe à retirer.</param>
    public void OnDamage<T>(T damageDealer, int damage, Vector2 knockBackForce)
    {
        PEat.fullness = Mathf.Clamp(PEat.fullness - damage, 0, 100);
        UpdatePlayerScale();

        //rb2D.AddForce(knockBackForce, ForceMode2D.Impulse);
        rb2D.velocity += Time.deltaTime * 100f * knockBackForce;
        PlayerState = PLAYER_STATE.KNOCKBACKED;
    }

    public void UpdatePlayerScale()
    {
        transform.localScale = Vector3.one * Mathf.Lerp(minSize, maxSize, pEat.fullness * .01f);
    }

    /// <summary>
    /// Pousse de façon smooth le joueur vers un endroit donné.
    /// </summary>
    public void MoveOverTo(Vector2 endPosition)
    {

        #if UNITY_EDITOR
        {
            CustomDebugs.DrawWireSphere(endPosition, .5f, Color.magenta, 5f, 1);
        }
        #endif

        StartCoroutine(MoverOverAnimation(endPosition));
    }

    IEnumerator MoverOverAnimation(Vector2 endPosition)
    {
        Vector2 iniPos = transform.position;
        float t = 0;
        while (t < 1)
        {
            if (PlayerState == PLAYER_STATE.KNOCKBACKED)
                break;

            rb2D.velocity = Vector2.zero;
            transform.position = DOVirtual.EasedValue(iniPos, endPosition, t, Ease.OutBack, 2f);
            t += Time.deltaTime * 4f;
            yield return null;
        }

        if(PlayerState != PLAYER_STATE.KNOCKBACKED)
        {
            transform.position = endPosition;
        }
    }
    #endregion
}
