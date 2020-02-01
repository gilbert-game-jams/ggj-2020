using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    Rigidbody _rigidBody;
    Transform _transform;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    private void FixedUpdate() {
        var velocityDir = _rigidBody.velocity.normalized;
        _transform.LookAt(_transform.position - velocityDir);
    }

    private void OnCollisionEnter(Collision other) {
        Destroy(gameObject);
    }
}
