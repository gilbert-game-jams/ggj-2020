using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowBehaviour : MonoBehaviour
{
    [FMODUnity.EventRef] public string hitCrack;
    Rigidbody _rigidBody;
    Transform _transform;

    private void Awake() {
        _rigidBody = GetComponent<Rigidbody>();
        _transform = GetComponent<Transform>();
    }

    private void FixedUpdate() {
        var velocityDir = _rigidBody.velocity.normalized;
        _transform.LookAt(_transform.position + velocityDir);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.GetComponent<CrackBehaviour>() != null)
        {
            //FMODUnity.RuntimeManager.PlayOneShot(hitCrack, transform.position);
        }
        if (other.gameObject.GetComponent<ArrowBehaviour>() == null) { 
            Destroy(gameObject);
        }
    }
}
