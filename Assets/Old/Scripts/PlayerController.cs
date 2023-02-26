using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool _isGrounded = false;

    private bool _facingRight = true;

    public Vector2 _velocity;

    private void Awake()
    {
        _playerCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        DetectingCollider();
        Move();
        Jump();
        transform.Translate(_velocity * Time.deltaTime);
        _velocity.y += _gravity * _gravityScale * Time.deltaTime;
    }

    #region Move

    [SerializeField] float _speed;
    [SerializeField] float _groundDeceleration;
    [SerializeField] float _airAcceleration;
    [SerializeField] float _walkAcceleration;

    public void Move()
    {
        float _moveInput = Input.GetAxisRaw("Horizontal");
        
        float acceleration = _isGrounded ? _walkAcceleration : _airAcceleration;

        float deceleration = _isGrounded ? _groundDeceleration : 0;

        if (_moveInput != 0)
        {
            _velocity.x = Mathf.MoveTowards(_velocity.x, _speed * _moveInput, acceleration * Time.deltaTime);

            if (_moveInput < 0 && _facingRight)
            {
                Flip();
            }

            if (_moveInput > 0 && !_facingRight)
            {
                Flip();
            }
        }
        else
            _velocity.x = Mathf.MoveTowards(_velocity.x, 0, deceleration * Time.deltaTime);
    }

    void Flip()
    {
        Vector3 currentScale = transform.localScale;

        currentScale.x *= -1f;

        gameObject.transform.localScale = currentScale;

        _facingRight = !_facingRight;
    }

    #endregion

    #region Detecting Collision

    BoxCollider2D _playerCollider;

    [SerializeField] LayerMask _groundMask;

    void DetectingCollider()
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, _playerCollider.size, 0, _groundMask);

        foreach (Collider2D hit in hits)
        {
            if (hit == _playerCollider)
            {
                continue;
            }

            ColliderDistance2D colliderDistance = hit.Distance(_playerCollider);

            if (colliderDistance.isOverlapped)
            {
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
            }

            if (Vector2.Angle(colliderDistance.normal, Vector2.up) < 90 && _velocity.y < 0)
            {
                _isGrounded = true;
            }
        }

        _isGrounded = false;
    }

    #endregion

    #region Jumping

    [SerializeField] float _jumpHeight = 5f;

    [SerializeField] float _gravity = -10f;

    [SerializeField] float _gravityScale = 1f;

    void Jump()
    {
        if (_isGrounded)
        {
            _velocity.y = 0;

            if (Input.GetButtonDown("Jump"))
            {
                _velocity.y = Mathf.Sqrt(2 * _jumpHeight * Mathf.Abs(_gravity * _gravityScale));

                Debug.Log("Hello");
            }
        }
    }
    #endregion
}
