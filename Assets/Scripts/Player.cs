using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }
      [SerializeField] private float playerSpeed = 10f;

    private void Awake() {
        Instance = this;
    }
    private void Update() {
        HandleMovement();
    }
    private void HandleMovement() {
        Vector2 movementInput = GameInput.Instance.GetPlayerMovementVectorNormalized();
        transform.position += new Vector3(movementInput.x, 0 , movementInput.y) * Time.deltaTime *playerSpeed;
    }

    public float GetPlayerSpeed(){
        return playerSpeed;
    }
}
