using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;

public class IKBoneTarget : MonoBehaviour {
    [SerializeField] private IKBoneTarget oppositeLeg;
    [SerializeField] private Transform body;
    [SerializeField] private IKTargetSettingsSO ikTargetSettings;
    [SerializeField] private bool drawGizmos;
    private float lerp = 0f;
    private bool lerpToOffset = false;
    //Current position of the leg
    private Vector3 currentPosition;
    //Addition vector added to the newPosition based on user input
    private Vector3 movementVector;
    //Offset vector from the body to the leg
    [SerializeField] private Vector3 offsetVector;
    private Vector3 originalOffsetVector;
    //New position of the leg
    private Vector3 newPosition;
    //Old position of the leg
    private Vector3 oldPosition;
    private Vector3 restPosition;
    private bool isMoving = false;
    private bool AllowMove = true;
    private bool rotate = false;

    private void Start() {
        offsetVector += transform.position - body.position;
        originalOffsetVector = offsetVector;
        currentPosition = transform.position;
        newPosition = transform.position;
        oldPosition = transform.position;
        movementVector = Vector3.zero;
        restPosition = body.transform.position;
        Player.Instance.OnRotate += Player_OnRotate;
    }

    private void Player_OnRotate(object sender, System.EventArgs e) {
        rotate = true;
    }

    // Update is called once per frame
    private void Update() {        transform.position = currentPosition;
        if (!isMoving) {
            if (rotate) {
                ResetPositionWithPlayerRotation();
            }
            //If the body's last movement was to go back to the rest position, make sure body has to get out of restRadius before allowing movement of IKLeg
            //if (lerpToOffset && !isMoving) {
            //    if (Vector3.Distance(body.position, restPosition) > ikTargetSettings.restRadius) {
            //        AllowMove = true;
            //    }
            //    else {
            //        AllowMove = false;
            //    }
            //}
            //Calculate the movementVector to add onto the raycastOrigin to make sure legs dont lag behind or go in front
            Vector2 inputVector = GameInput.Instance.GetPlayerMovementVectorNormalized();
            if (inputVector == Vector2.zero) {
                movementVector = Vector3.zero;
               
            }
            else {
                movementVector = new Vector3(inputVector.x, 0, inputVector.y);
                movementVector *= ikTargetSettings.globalMovementVectorMultiplier;
            }

            Vector3 rayOrigin = body.position + offsetVector + movementVector;
            rayOrigin.y += ikTargetSettings.rayVerticalOffset;
            Ray ray = new Ray(rayOrigin, Vector3.down);

            if (AllowMove) {
                //Allow new raycast and reset of position if the IK is not moving or the player changed direction

                if (Physics.Raycast(ray, out RaycastHit info, ikTargetSettings.rayVerticalOffset + 3)) {
                    //If the player has moved a certain distance from the old position, the leg will move to the new position
                    if (Vector3.Distance(newPosition, info.point) > ikTargetSettings.stepDistance) {
                        newPosition = info.point;
                        lerp = 0f;
                        lerpToOffset = false;
                    }
                    //If the player has stood still for a while this condition will run
                    if (inputVector == Vector2.zero && !lerpToOffset) {
                        newPosition = info.point;
                        lerp = 0f;
                        lerpToOffset = true;
                        restPosition = body.position;
                    }
                }
            }
        }
        //Animation of leg
        if (lerp < 1f && (oppositeLeg.IsGrounded()) ) {
            isMoving = true;
            Vector3 footPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            footPosition.y = ikTargetSettings.stepHeightCurve.Evaluate(lerp) * ikTargetSettings.stepHeight;
            currentPosition = footPosition;
            lerp += Time.deltaTime * ikTargetSettings.speed;
        }
        if (lerp > 1f) {
            isMoving = false;
            currentPosition = newPosition;
            oldPosition = newPosition;
            
        }
    }

    public bool IsGrounded() {
        return !isMoving;
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
    private void ResetPositionWithPlayerRotation() {
        oldPosition = transform.position;
        if (oppositeLeg.IsGrounded()) {
            isMoving = true;
            rotate = false;
        }
        lerp = 0f;
        Vector3 rotatedPosition = RotatePointAroundPivot(body.transform.position + originalOffsetVector, body.transform.position, Player.Instance.transform.localEulerAngles);
        offsetVector = rotatedPosition - body.transform.position;
        newPosition = rotatedPosition;
    }
    private void OnDrawGizmos() {
            //Gizmos.color = UnityEngine.Color.red;
            //Gizmos.DrawSphere(oldPosition, 1f);
            //Gizmos.color = UnityEngine.Color.green;
            //Gizmos.DrawSphere(newPosition, 1f);
            //Gizmos.color = UnityEngine.Color.magenta;
        
    }
}
