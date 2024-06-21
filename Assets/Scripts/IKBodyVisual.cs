using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBodyVisual : MonoBehaviour {
    [SerializeField] private Transform[] IKTargetsLeft;
    [SerializeField] private Transform[] IKTargetsRight;
    [SerializeField] private Vector3 offset;
    [SerializeField] private float blend = 0.1f;
    private Vector3 averageTransformLeft;
    private Vector3 averageTransformRight;
    private Vector3 averageTransform;
    private Vector3 originalRotation;
    private Vector3 newRotation;
    private Vector3 tilt = new Vector3(0, 0, 0);
    private void Start() {
        originalRotation = transform.localEulerAngles;
    }
    private void Update() {
        //Calculate Average transforms of IKTargets on left and right
        averageTransformLeft = Vector3.zero;
        averageTransformRight = Vector3.zero;
        for (int i = 0; i < IKTargetsLeft.Length; i++) {
            averageTransformLeft += IKTargetsLeft[i].position;
        }
        averageTransformLeft /= IKTargetsLeft.Length;
        for (int i = 0; i < IKTargetsRight.Length; i++) {
            averageTransformRight += IKTargetsRight[i].position;
        }
        averageTransformRight /= IKTargetsRight.Length;
        averageTransform = (averageTransformLeft + averageTransformRight) / 2;

        //Calculate tilt by getting the difference between Left and Right Averages
        tilt = averageTransformRight - averageTransformLeft;

        Vector3 newRotation = originalRotation + tilt;
        //Tilt the body based on the new rotation
        transform.localRotation = Quaternion.Euler(newRotation);

        //Move the body to the average position of the IKTargets
        //Interpolate between previous position and new average position at 10%
        //Basically damp the effect

        Vector3 newTransform = Vector3.Lerp(transform.position, averageTransform + offset, blend);
        transform.position = new Vector3(transform.position.x, newTransform.y, transform.position.z);

    }

}
