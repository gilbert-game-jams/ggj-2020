using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BowBehaviour : MonoBehaviour
{
    [FMODUnity.EventRef] public string shootNoodleEvent;
    private FMOD.Studio.EventInstance shootNoodleInstance;
    private bool drawingBow;


    public GameObject _arrow;
    public GameObject _arrowSpawn;
    public Slider _slider;


    [Range(1, 50f)]
    public float _minimumArrowSpeed = 5f;

    [Range(1, 50f)]
    public float _maximumArrowSpeed = 10f;
    
    [Range(0.1f, 10f)]
    public float _maximumDrawTime = 2f;

    float _drawTime = 0.0f;
    float _drawFactor = 0.0f;

    private void OnValidate() 
    {
        _maximumArrowSpeed = _minimumArrowSpeed > _maximumArrowSpeed ? _minimumArrowSpeed : _maximumArrowSpeed;
    }

    float GetArrowSpeed(float minSpeed, float maxSpeed, float draw) {
        draw = Mathf.Clamp(draw, 0, 1);
        return Mathf.Lerp(minSpeed, maxSpeed, draw);
    }

    void FireArrow(float drawFactor, float maxSpeed)
    {
        shootNoodleInstance.setParameterValue("BowChargeRelease", 1);

        var arrow = GameObject.Instantiate(_arrow, _arrowSpawn.transform.position, _arrow.transform.rotation);

        // arrow.GetComponent<Rigidbody>().velocity = -transform.forward * maxSpeed * drawFactor;
        arrow.GetComponent<Rigidbody>().velocity = -transform.forward * GetArrowSpeed(_minimumArrowSpeed, _maximumArrowSpeed, drawFactor);
        
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
            drawingBow = false;
        } else if (Input.GetMouseButton(0)) {
            if (!drawingBow)
            {
                shootNoodleInstance = FMODUnity.RuntimeManager.CreateInstance(shootNoodleEvent);
                shootNoodleInstance.start();
                shootNoodleInstance.setParameterValue("BowChargeRelease", 0);
            }
            drawingBow = true;
            _drawTime = _drawTime < _maximumDrawTime ? _drawTime + Time.deltaTime : _maximumDrawTime;
            _drawFactor = _drawTime / _maximumDrawTime;
        }
        _slider.value = _drawFactor;
    }
}
