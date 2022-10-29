using System;
using UnityEngine;
using Data;

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
    public PLAYER_STATE PlayerState { get; set; }
    [HideInInspector] public Color color;

    //============================
    public Vector2 AimDirection { get; set; }

    //============================
    [SerializeField] private float tailleMax = 2.857f;
    [SerializeField] private float tailleMin = 1f;

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
        transform.localScale = Vector3.one * Mathf.Lerp(tailleMin, tailleMax, pEat.Remplissage * .01f);
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
    #endregion
}
