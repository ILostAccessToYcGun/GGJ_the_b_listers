using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovement;
    [SerializeField] PlayerLance playerLance;
    [SerializeField] PlayerCamera playerCamera;

    private PlayerInput _inputActions;

    void Start()
    {
        _inputActions = new PlayerInput();
        _inputActions.Enable();

        playerMovement.Init();
        playerLance.Init();
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
        };

        playerMovement.Runtime(characterInput);
        playerCamera.Runtime(playerMovement.gameObject.transform.position);

        /*
        //Get camera input and update Cam Rotation
        var cameraInput = new CameraInput { Look = input.Look.ReadValue<Vector2>() };
        playerCamera.UpdateRotation(cameraInput);

        //Get character input and update it
        var characterInput = new CharacterInput
        {
            Rotation = playerCamera.transform.rotation,
            Move = input.Move.ReadValue<Vector2>(),
            Jump = input.Jump.WasPressedThisFrame(),
            JumpSustain = input.Jump.IsPressed(),
            Dash = input.Dash.WasPressedThisFrame(),
            Sprint = input.Sprint.IsPressed()
        };
        playerCharacter.UpdateInput(characterInput);
        playerCharacter.UpdateBody(deltaTime);
        playerCharacter.BoostVisual(deltaTime);

        var combatInput = new CombatInput
        {
            Shoot = input.Attack.IsPressed(),
            Reload = input.Reload.WasPressedThisFrame()
        };
        playerGun.RotateGunTowardsMouse();
        if (!playerUIToggler.GetUIOpenBool())
            playerGun.UseWeapon(combatInput);
         */
    }
}
