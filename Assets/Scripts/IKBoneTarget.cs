using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBoneTarget : MonoBehaviour {
    [SerializeField] private IKBoneTarget oppositeLeg;
    [SerializeField] private Transform body;
    [SerializeField] private IKTargetSettingsSO ikTargetSettings;
    private float lerp = 1f;
    //Current position of the leg
    private Vector3 currentPosition;
    //Addition vector added to the newPosition based on user input
    private Vector3 additionVector;
    //Offset vector from the body to the leg
    [SerializeField] private Vector3 offsetVector;
    //New position of the leg
    private Vector3 newPosition;
    //Old position of the leg
    private Vector3 oldPosition;
    private bool isMoving = false;


    private void Start() {
        offsetVector += transform.position - body.position;
        currentPosition = transform.position;
        newPosition = transform.position;
        oldPosition = transform.position;
        additionVector = Vector3.zero;
        
    }

    // Update is called once per frame
    private void Update() {
        transform.position = currentPosition;
        Vector2 inputVector = GameInput.Instance.GetPlayerMovementVectorNormalized();
        Vector3 inputVector3 = new Vector3(inputVector.x, 0, inputVector.y);
        if (inputVector3 != additionVector && inputVector3 != Vector3.zero) {
            additionVector = inputVector3;
            additionVector *= ikTargetSettings.stepDistance;
        }
        Ray ray = new Ray(body.position + offsetVector + new Vector3(0, 1, 0), Vector3.down);

        if (!isMoving)
            if (Physics.Raycast(ray, out RaycastHit info, 10)) {
                info.point -= new Vector3(0, 1, 0);
                if (Vector3.Distance(newPosition, info.point) > ikTargetSettings.stepDistance) {
                    newPosition = info.point + additionVector;
                    lerp = 0;
                }
            }
        if (lerp < 1f && oppositeLeg.IsGrounded()) {
            isMoving = true;
            Vector3 footPosition = Vector3.Lerp(oldPosition, newPosition, lerp);
            footPosition.y += Mathf.Sin(lerp * Mathf.PI) * ikTargetSettings.stepHeight;
            currentPosition = footPosition;
            lerp += Time.deltaTime * ikTargetSettings.speed;
        }
        else if(lerp>1f) {
            isMoving = false;
            oldPosition = newPosition;
        }
    }

    public bool IsGrounded() {
        return !isMoving;
    }
    //private void OnDrawGizmos() {

    //    Gizmos.color = Color.green;
    //    Gizmos.DrawSphere(newPosition, 1f);
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(raycastPosition, 1f);
    //}
}
