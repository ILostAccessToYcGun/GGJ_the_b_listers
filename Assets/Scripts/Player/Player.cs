using UnityEngine;
using UnityEngine.InputSystem;

public struct CharacterState
{
    public bool Firing;
    public bool Lancing;
    public Vector3 Velocity;
    public Vector3 Acceleration;
}

public struct CharacterInput
{
    public bool Thrust;
    public bool AfterBurner;
    public bool Shoot;
}

public struct CameraInput
{
    public Vector2 Look;
}

public class Player : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerWeaponry playerWeaponry;
    [SerializeField] PlayerCamera playerCamera;

    private PlayerInput _inputActions;

    void Start()
    {
        _inputActions = new PlayerInput();
        _inputActions.Enable();

        playerMovement.Init();
        playerWeaponry.Init();
        playerCamera.Init();
    }

    void OnDestroy()
    {
        _inputActions.Dispose();
    }

    void Update()
    {
        var input = _inputActions.Player;
        var ui = _inputActions.UI;

        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };

        var characterInput = new CharacterInput
        {
            Thrust = input.Thrust.IsPressed(),
            AfterBurner = input.Sprint.IsPressed(),
            Shoot = input.Shoot.IsPressed(),
        };

        playerMovement.Runtime(characterInput);
        playerCamera.Runtime(playerMovement.gameObject.transform.position);
        playerWeaponry.Runtime(characterInput);

    }
}
