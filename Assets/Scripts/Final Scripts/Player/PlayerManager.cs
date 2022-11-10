using System;
using System.Collections;
using System.Collections.Generic;
using Data;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private PlayerInputHandler _inputs;
    private Rigidbody2D _rb;

    [field: Header("Player State")]
    public PlayerState State { get; private set; }

    public Vector2 InputVector { get; private set; }

    public float eatAmount;

    private const float maxEatValue = 1;
    public float MaxEatValue => maxEatValue;

    private const float maxScale = 2.857143f;

    public float MaxScale => maxScale;
    
    public SwitchSizeSkin SwitchSkin  { get; private set; }
    
    public Image imageUI;
    public TextMeshProUGUI textUI;

    private bool _canEat;

    public bool CanEat => _canEat;

    private void Awake()
    {
        eatAmount = MaxEatValue/2;
        _inputs = GetComponent<PlayerInputHandler>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        State = PlayerState.Moving;
    }

    public void SetInputVector(Vector2 direction)
    {
        InputVector = direction;
    }

    public void SetPlayerState(PlayerState state)
    {
        State = state;
    }

    public void SetSkin(SwitchSizeSkin skin)
    {
        SwitchSkin = skin;
    }

    public void SetCanEat(bool result)
    {
        _canEat = result;
    }

    public void EnableInputs()
    {
        Debug.Log("Enable");
        _inputs.EnableInputs();
    }
    public void DisableInputs()
    {
        Debug.Log("Disable");
        _inputs.DisableInputs();
    }

    public void ResetPlayer()
    {
        eatAmount = MaxEatValue/2;
    }

    [ContextMenu("SetDead")]
    public void SetDead()
    {
        State = PlayerState.Dead;
        //GameManager.Instance.gameObject.GetComponent<RespawnPlayer>().Respawn(gameObject);
    }

    [ContextMenu("SetDead2")]
    public void SetDead2()
    {
        State = PlayerState.Dead;
        GameManager.Instance.RespawnPlayer(gameObject);
    }

    public void OnDamage<T>(T damageDealer, int damage, Vector2 knockBackForce)
    {
        eatAmount -= damage; //Div damage par 100

        _rb.AddForce(knockBackForce, ForceMode2D.Impulse);
        State = PlayerState.KNOCKBACKED;
    }

    public void OnDamage<T>(T damageDealer, int damage)
    {
        eatAmount -= damage; //Div damage par 100
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
            if (State == PlayerState.KNOCKBACKED)
                break;

            _rb.velocity = Vector2.zero;
            transform.position = DOVirtual.EasedValue(iniPos, endPosition, t, Ease.OutBack, 2f);
            t += Time.deltaTime * 4f;
            yield return null;
        }

        if (State == PlayerState.KNOCKBACKED)
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
}
