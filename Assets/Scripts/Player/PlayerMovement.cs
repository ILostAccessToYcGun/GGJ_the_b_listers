using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement Physics")]
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float engineForce = 3500f;
    [SerializeField] private float linearDrag = 1f;
    [SerializeField] private float chargingDrag = 4f; 

    [Space]
    [Range(1f, 4f)]
    [SerializeField] private float afterburnerMultiplier = 1.5f;
    [Range(0f, 2f)]
    [SerializeField] private float idleGravity = 0.3f;
    [Space]
    [SerializeField] private float boostMax;

    private float boostAmount;

    private Vector2 _lookDirection;
    private bool _isThrusting;
    private bool _isCharging; 
    private bool _gravityOn;
    private Camera _cam;

    public void Init()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        _cam = Camera.main;

        rb.linearDamping = linearDrag;

        boostAmount = boostMax;
    }

    public void Runtime(CharacterInput characterInput)
    {
        if (_cam == null) _cam = Camera.main;

        _isCharging = characterInput.Lance;

        _isThrusting = characterInput.Thrust && !_isCharging;

        Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        _lookDirection = (mousePos - transform.position).normalized;

        if (_lookDirection != Vector2.zero)
        {
            transform.up = _lookDirection;
        }

        rb.linearDamping = _isCharging ? chargingDrag : linearDrag;

        float speedMultiplier = characterInput.AfterBurner ? afterburnerMultiplier : 1f;
        float currentMaxSpeed = maxSpeed * speedMultiplier;

        if (_isThrusting)
        {
            _gravityOn = false;
            rb.AddForce(_lookDirection * engineForce * speedMultiplier * Time.deltaTime);
        }
        else
        {
            _gravityOn = true;
        }

        rb.gravityScale = _gravityOn ? idleGravity : 0f;


        if (rb.linearVelocity.magnitude > currentMaxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentMaxSpeed;
        }
    }
}