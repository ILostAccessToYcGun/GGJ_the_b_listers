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
    South,
    East,
    West,
    NorthEast,
    NorthWest,
    SouthEast,
    SouthWest,
}

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [Range(1f, 2f)]
    [SerializeField] private float afterburnerMultiplier;
    [Header("Visuals")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializedDictionary("Sprite, Cardinal Direction")]
    [SerializeField] public SerializedDictionary<Sprite, Pose> quadrant1Poses;

    private CharacterState _state;
    private CharacterState _lastState;


    public void Init()
    {
        
    }

    public void Runtime()
    {
        
    }
}
