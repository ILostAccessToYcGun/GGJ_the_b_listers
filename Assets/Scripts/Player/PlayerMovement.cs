using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterState
{
    public bool Charging;
    public Vector3 Velocity;
    public Vector3 Acceleration;
}

public struct CharacterInput
{
    public Quaternion Rotation;
    public bool Thrust;
}

public enum Pose
{
    North,
    NorthEast,
    East,
    SouthEast,
    South,
    SouthWest,
    West,
    NorthWest
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Compnents")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [Range(1f, 2f)]
    [SerializeField] private float afterburnerMultiplier = 1.5f;
    [Space]
    [Range(0f, 1f)]
    [SerializeField] private float idleGravity = 0.3f;

    [Header("Visuals")]
    [SerializedDictionary("Cardinal Direction", "Sprite")]
    [SerializeField] public SerializedDictionary<Pose, Sprite> quadrant1Poses;

    private CharacterState _state;
    private Vector2 _lookDirection;
    private bool _isThrusting;
    private bool _gravityOn;
    private Camera _cam;

    public void Init()
    {
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        _cam = Camera.main;
    }

    public void Runtime()
    {
        if (_cam == null) _cam = Camera.main;
        _isThrusting = Input.GetKey(KeyCode.W);

        Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        _lookDirection = (mousePos - transform.position).normalized;

        if (_lookDirection.sqrMagnitude > 0.001f)
        {
            float angle = Vector2.SignedAngle(Vector2.up, _lookDirection);
            DecideSprite(angle);
        }

        if (_isThrusting)
        {
            float speed = Input.GetKey(KeyCode.LeftShift) ? moveSpeed * afterburnerMultiplier : moveSpeed;
            _state.Velocity = _lookDirection * speed;
            transform.position += _state.Velocity * Time.deltaTime;

            _gravityOn = false;

            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            _gravityOn = true;
        }

        rb.gravityScale = _gravityOn ? idleGravity : 0f;
    }

    void DecideSprite(float signedAngle)
    {
        float angle = signedAngle;
        if (angle < 0) angle += 360f;

        // Snap to 45 degree increments
        int sector = Mathf.FloorToInt((angle + 22.5f) / 45f);

        if (sector >= 8) sector = 0;

        Pose currentPose = (Pose)sector;
        UpdateRenderer(currentPose);
    }

    private void UpdateRenderer(Pose pose)
    {
        bool flipX = false;
        bool flipY = false;
        Sprite spriteToUse = null;

        switch (pose)
        {
            case Pose.North:
                quadrant1Poses.TryGetValue(Pose.North, out spriteToUse);
                break;

            case Pose.NorthEast:
                flipX = true;
                quadrant1Poses.TryGetValue(Pose.NorthEast, out spriteToUse);
                break;

            case Pose.East:
                quadrant1Poses.TryGetValue(Pose.East, out spriteToUse);
                flipX = true;
                break;

            case Pose.SouthEast:
                quadrant1Poses.TryGetValue(Pose.NorthEast, out spriteToUse);
                flipY = true;
                break;

            case Pose.South:
                quadrant1Poses.TryGetValue(Pose.North, out spriteToUse);
                flipY = true;
                break;

            case Pose.SouthWest:
                quadrant1Poses.TryGetValue(Pose.NorthEast, out spriteToUse);
                flipX = true;
                flipY = true;
                break;

            case Pose.West:
                quadrant1Poses.TryGetValue(Pose.East, out spriteToUse);
                break;

            case Pose.NorthWest:
                quadrant1Poses.TryGetValue(Pose.NorthEast, out spriteToUse);
                flipX = false;
                break;
        }

        if (spriteToUse != null)
        {
            spriteRenderer.sprite = spriteToUse;
            spriteRenderer.flipX = flipX;
            spriteRenderer.flipY = flipY;
        }
    }
}