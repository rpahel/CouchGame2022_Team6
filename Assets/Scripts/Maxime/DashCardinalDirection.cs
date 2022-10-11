using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class DashCardinalDirection : MonoBehaviour
{
    private Raf_PLayer rafPlayer;
    private TrailRenderer _trailRenderer;
  //  private CharacterController chacC;
    private Rigidbody2D rb;

    [Header("Dashing")]
    [SerializeField] private float _dashingVelocity = 14f;
    [SerializeField] private float _dashingTime = 0.5f;
    private Vector2 _dashingDir;
    private bool _isDashing;
    //[HideInInspector]
    public bool _canDash = true;

    public Vector2 aimingDir;
    private Vector2 aimingPos;
    public Transform pointeur;       // Le Parent du viseur pour la rotation du "bras"
    public Transform pointeurBase;   // Point de départ des raycasts
    private float angle;


    private void Awake()
    {
        pointeur.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        rafPlayer = GetComponent<Raf_PLayer>();
        // chacC = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody2D>();
        _trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        aimingDir = aimingPos.normalized;
        angle = Mathf.Atan2(aimingDir.y, aimingDir.x);

        dash();
      
        
    }

    private void FixedUpdate()
    {
        pointeur.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * angle);
    }


    void dash()
    {
        var dashInput = Input.GetButtonDown("Dash");


        if (dashInput && _canDash)
        {
            _isDashing = true;

            _trailRenderer.emitting = true;

            //ici faut juste mettre le vector de direction du dash
            _dashingDir = aimingDir;


            if (_dashingDir == Vector2.zero)
            {
                _dashingDir = new Vector2(transform.localScale.x, 0);
            }
            StartCoroutine(StopDashing());
        }

        if (_isDashing)
        {
            rb.velocity = _dashingDir.normalized * _dashingVelocity;

            return;

        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(_dashingTime);
        _trailRenderer.emitting = false;
        _isDashing=false;

    }

    public void GetAiming(InputAction.CallbackContext context)
    {
        aimingPos = context.ReadValue<Vector2>();
        pointeur.gameObject.SetActive(aimingPos.sqrMagnitude > 0.1f ? true : false);
    }

   
}
