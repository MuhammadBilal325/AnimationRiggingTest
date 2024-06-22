using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    public event EventHandler OnRotate;
    public static Player Instance { get; private set; }
    [SerializeField] private float playerSpeed = 10f;
    [SerializeField] private Cinemachine.CinemachineFreeLook freeLookCamera;
    private Vector3 lookAtPoint;

    private void Awake() {
        Instance = this;
    }
    private void Start() {
        GameInput.Instance.OnRotateViewInput += GameInput_OnRotateViewInput;
    }

    private void GameInput_OnRotateViewInput(object sender, System.EventArgs e) {
        HandleRotation();   
    }

    private void Update() {
        HandleMovement();
    }
    private void HandleMovement() {
        Vector2 movementInput = GameInput.Instance.GetPlayerMovementVectorNormalized();
        transform.position += new Vector3(movementInput.x, 0 , movementInput.y) * Time.deltaTime *playerSpeed;
    }
    private void HandleRotation() {
        lookAtPoint = freeLookCamera.transform.position - transform.position;
        lookAtPoint *= -1;
        lookAtPoint.y = transform.position.y;
        lookAtPoint+= transform.position;
        transform.LookAt(lookAtPoint);
        OnRotate?.Invoke(this, EventArgs.Empty);
    }

    public float GetPlayerSpeed(){
        return playerSpeed;
    }
    //private void OnDrawGizmos() {
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(lookAtPoint,1f);
   // }
}
