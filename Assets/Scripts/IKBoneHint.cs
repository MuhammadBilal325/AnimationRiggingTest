using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBoneHint : MonoBehaviour {
    [SerializeField] private Transform body;
    [SerializeField] private IKTargetSettingsSO ikTargetSettings;
    private float lerp = 0f;
    private Vector3 oldPosition;
    private Vector3 newPosition;
    private Vector3 offsetVector;
    private Vector3 originalOffsetVector;
    private bool rotating = false;
    private void Start() {
        offsetVector = transform.position - body.position;
        originalOffsetVector = offsetVector;
        Player.Instance.OnRotate += Player_OnRotate;
    }

    private void Player_OnRotate(object sender, System.EventArgs e) {
        ResetPositionWithPlayerRotation();
    }

    private void Update() {
        if (rotating) {
            if (lerp < 1f) {
                transform.position = Vector3.Lerp(oldPosition, newPosition, lerp);
                lerp += Time.deltaTime * ikTargetSettings.speed;
            }
            else {
                transform.position = newPosition;
                offsetVector = newPosition - body.transform.position;
                rotating = false;
            }
        }
        else {
            transform.position = body.position + offsetVector;
        }
    }

    Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles) {
        Vector3 dir = point - pivot; // get point direction relative to pivot
        dir = Quaternion.Euler(angles) * dir; // rotate it
        point = dir + pivot; // calculate rotated point
        return point; // return it
    }
    private void ResetPositionWithPlayerRotation() {
        Vector3 rotatedPosition = RotatePointAroundPivot(body.transform.position + originalOffsetVector, body.transform.position, Player.Instance.transform.localEulerAngles);
        oldPosition = transform.position;
        newPosition = rotatedPosition;
        offsetVector = rotatedPosition - body.transform.position;
        lerp = 0f;
        rotating = true;
    }
}

