using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 3f;

    private Rigidbody2D _rb;
    private Vector2 _inputVector = Vector2.zero;

    private void Awake()
    {
        _rb = gameObject.GetComponent<Rigidbody2D>();
    }
    
    public void SetInputVector(Vector2 direction)
    {
        _inputVector = direction;
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        _rb.velocity = new Vector2(_inputVector.x * moveSpeed, 0 );
    }
}