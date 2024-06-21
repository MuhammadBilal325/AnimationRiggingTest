using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKBoneHint : MonoBehaviour {
    [SerializeField] private Transform body;
    private Vector3 offsetVector;

    private void Start() {
        offsetVector = transform.position - body.position;
    }

    private void Update() {
        transform.position = body.position + offsetVector;
    }
}

