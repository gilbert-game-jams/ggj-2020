using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowBehaviour : MonoBehaviour
{
    public GameObject _arrow;
    public GameObject _arrowSpawn;
    public Slider _slider;
    
    [Range(1, 20f)]
    public float _maximumArrowSpeed = 10f;
    
    [Range(0.1f, 10f)]
    public float _maximumDrawTime = 2f;

    float _drawTime = 0.0f;
    float _drawFactor = 0.0f;

    void FireArrow(float drawFactor, float maxSpeed)
    {
        var arrow = GameObject.Instantiate(_arrow, _arrowSpawn.transform.position, _arrow.transform.rotation);
        arrow.GetComponent<Rigidbody>().velocity = -transform.forward * maxSpeed * drawFactor;
        var arrowSpawnTrans = _arrowSpawn.transform;
        arrow.transform.LookAt(arrowSpawnTrans.position + arrowSpawnTrans.forward, arrowSpawnTrans.up);
    }

    void ResetDraw() {
        _drawFactor = 0.0f;
        _drawTime = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0)) 
        {
            FireArrow(_drawFactor, _maximumArrowSpeed);
            ResetDraw();
        } else if (Input.GetMouseButton(0)) {
            _drawTime = _drawTime < _maximumDrawTime ? _drawTime + Time.deltaTime : _maximumDrawTime;
            _drawFactor = _drawTime / _maximumDrawTime;
        }
        _slider.value = _drawFactor;
    }
}
