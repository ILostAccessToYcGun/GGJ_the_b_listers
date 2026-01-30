using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterInput
{
    public bool Thrust;
}

public struct CameraInput
{
    public Vector2 Look;
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
    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rb;

    [Header("Movement Physics")]
    [SerializeField] private float maxSpeed = 12f;
    [SerializeField] private float engineForce = 3500f;
    [SerializeField] private float linearDrag = 1f;

    [Space]
    [Range(1f, 4f)]
    [SerializeField] private float afterburnerMultiplier = 1.5f;
    [Range(0f, 2f)]
    [SerializeField] private float idleGravity = 0.3f;

    [Header("Visuals")]
    [SerializedDictionary("Cardinal Direction", "Sprite")]
    [SerializeField] public SerializedDictionary<Pose, Sprite> quadrant1Poses;
    [SerializeField] public Sprite shootingPose;

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

        rb.linearDamping = linearDrag;
    }

    public void Runtime(CharacterInput characterInput)
    {
        if (_cam == null) _cam = Camera.main;
        _isThrusting = characterInput.Thrust;

        Vector3 mousePos = _cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        _lookDirection = (mousePos - transform.position).normalized;

        if (_lookDirection.sqrMagnitude > 0.001f)
        {
            float angle = Vector2.SignedAngle(Vector2.up, _lookDirection);
            DecideSprite(angle);
        }

        rb.linearDamping = linearDrag;

        float speedMultiplier = Input.GetKey(KeyCode.LeftShift) ? afterburnerMultiplier : 1f;
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

    void DecideSprite(float signedAngle)
    {
        float angle = signedAngle;
        if (angle < 0) angle += 360f;

        // Snap to 45 degree increments
        int sector = Mathf.FloorToInt((angle + 22.5f) / 45f);

        if (sector >= 8) sector = 0;

        if (!_state.Firing)
        {
            Pose currentPose = (Pose)sector;
            UpdateRenderer(currentPose);
        }
        else
        {
            spriteRenderer.sprite = shootingPose;
        }
        
    }

    private void UpdateRenderer(Pose pose)
    {
        bool flipX = false;
        bool flipY = false;
        Sprite spriteToUse = null;

        switch (pose)
        {
            //todo, swap to rotating, not flipping (at least for q4)
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