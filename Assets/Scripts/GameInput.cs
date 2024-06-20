using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInput : MonoBehaviour
{
    public event EventHandler OnMovementInput;
    public static GameInput Instance { get; private set; }
    private PlayerInputActions playerInputActions;
    private void Awake() {
        Instance = this;
        playerInputActions = new PlayerInputActions();
        playerInputActions.Player.MovementWASD.performed += MovementWASD_performed;
        playerInputActions.Player.Enable();
    }

    private void MovementWASD_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj) {
        OnMovementInput?.Invoke(this, EventArgs.Empty);
    }


    public Vector2 GetPlayerMovementVector() {
        return playerInputActions.Player.MovementWASD.ReadValue<Vector2>();
    
    }
    public Vector2 GetPlayerMovementVectorNormalized() {
        return playerInputActions.Player.MovementWASD.ReadValue<Vector2>().normalized;
    }
}
