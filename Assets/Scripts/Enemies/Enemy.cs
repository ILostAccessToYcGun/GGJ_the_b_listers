using UnityEngine;
using UnityEngine.InputSystem;

public class Enemy : MonoBehaviour
{
    //this will be the main script that runs other components
    //like movement, shooting, vision

    [SerializeField] EnemyMovement movement;
    [SerializeField] WeaponBase weapon;


    void Start()
    {

    }

    void OnDestroy()
    {
        
    }

    void Update()
    {
        weapon.Fire();

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
