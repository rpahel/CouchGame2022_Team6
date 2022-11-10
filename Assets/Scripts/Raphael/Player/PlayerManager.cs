using System;
using UnityEngine;
using Data;
using System.Collections;
using DG.Tweening;

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

    // Getter
    public Rigidbody2D Rb2D => rb2D;
    public Collider2D PCollider => coll;
    public PlayerMovement PMovement => pMovement;
    public PlayerInputs PInputs => pInputs;
    public PlayerShoot PShoot => pShoot;
    public PlayerEat PEat => pEat;
    #endregion

    #region Variables
    //============================
    [HideInInspector] public Color color;

    //============================
    public PLAYER_STATE PlayerState { get; set; }
    public Vector2 AimDirection { get; set; }

    //============================
    [SerializeField] private float tailleMax = 2.857f;
    [SerializeField] private float tailleMin = 1f;

    //============================ TODO : Supprimer
    public SpriteRenderer spriteInterieur;
    #endregion

    #region Unity_Functions
    private void Awake()
    {
        TryGetAllComponents();
        SetManagerInComponents();
    }

    private void Update()
    {
        #if UNITY_EDITOR
            Debug.DrawRay(transform.position - Vector3.forward, AimDirection * 5f, Color.cyan, Time.deltaTime);
        #endif
    }

    private void FixedUpdate()
    {
        #if UNITY_EDITOR
        transform.localScale = Vector3.one * Mathf.Lerp(tailleMin, tailleMax, pEat.Remplissage * .01f);

        Debug.DrawRay(transform.position - Vector3.forward, AimDirection * 5f, Color.cyan, Time.deltaTime);
        
        if (PlayerState == PLAYER_STATE.KNOCKBACKED)
        {
            spriteInterieur.color = Color.red;
        }
        else if (PlayerState == PLAYER_STATE.SHOOTING)
        {
            spriteInterieur.color = Color.blue;
        }
        else
        {
            spriteInterieur.color = Color.white;
        }
        #endif
    }
    #endregion

    #region Custom_Functions
    /// <summary>
    /// Essaie de récupérer, dans le Player gameObject, le Component donné. Arrête le mode Play et retourne une erreur lorsque le Component n'est pas trouvé.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component">Le component à récupérer.</param>
    /// <exception cref="Exception">Le component n'a pas été trouvé</exception>
    private void TryGetPlayerComponent<T>(out T component)
    {
        if (!TryGetComponent<T>(out component))
        {
            #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
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
    }

    private void SetManagerInComponents()
    {
        pMovement.PManager = this;
        pEat.PManager = this;
        pInputs.PManager = this;
        pShoot.PManager = this;
    }

    public void OnDamage<T>(T damageDealer, int damage, Vector2 knockBackForce)
    {
        PEat.Remplissage -= damage;
        PEat.Remplissage = Mathf.Clamp(PEat.Remplissage, 0, 100);
        UpdatePlayerScale();

        rb2D.AddForce(knockBackForce, ForceMode2D.Impulse);
        PlayerState = PLAYER_STATE.KNOCKBACKED;
    }

    public void UpdatePlayerScale()
    {
        transform.localScale = Vector3.one * Mathf.Lerp(tailleMin, tailleMax, pEat.Remplissage * .01f);
    }

    public void PousseToiVers(Vector2 endPosition)
    {
        DrawWireSphere(endPosition, .5f, Color.cyan, 5f);
        StartCoroutine(PousseToiAnimation(endPosition));
    }

    IEnumerator PousseToiAnimation(Vector2 endPosition)
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

    public static void DrawWireSphere(Vector3 center, float radius, Color color, float duration, int quality = 3)
    {
        quality = Mathf.Clamp(quality, 1, 10);
    
        int segments = quality << 2;
        int subdivisions = quality << 3;
        int halfSegments = segments >> 1;
        float strideAngle = 360F / subdivisions;
        float segmentStride = 180F / segments;
    
        Vector3 first;
        Vector3 next;
        for (int i = 0; i < segments; i++)
        {
            first = (Vector3.forward * radius);
            first = Quaternion.AngleAxis(segmentStride * (i - halfSegments), Vector3.right) * first;
    
            for (int j = 0; j < subdivisions; j++)
            {
                next = Quaternion.AngleAxis(strideAngle, Vector3.up) * first;
                UnityEngine.Debug.DrawLine(first + center, next + center, color, duration);
                first = next;
            }
        }
    
        Vector3 axis;
        for (int i = 0; i < segments; i++)
        {
            first = (Vector3.forward * radius);
            first = Quaternion.AngleAxis(segmentStride * (i - halfSegments), Vector3.up) * first;
            axis = Quaternion.AngleAxis(90F, Vector3.up) * first;
    
            for (int j = 0; j < subdivisions; j++)
            {
                next = Quaternion.AngleAxis(strideAngle, axis) * first;
                UnityEngine.Debug.DrawLine(first + center, next + center, color, duration);
                first = next;
            }
        }
    }
    #endregion
}
