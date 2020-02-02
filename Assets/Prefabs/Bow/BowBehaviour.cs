using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class BowBehaviour : MonoBehaviour
{
    public Animator _animator;
    [FMODUnity.EventRef] public string shootNoodleEvent;
    private FMOD.Studio.EventInstance shootNoodleSoundEvent;

    public GameObject _arrow;
    public GameObject _arrowSpawn;


    [Range(1, 50f)]
    public float _minArrowSpeed = 5f;

    [Range(1, 50f)]
    public float _maxArrowSpeed = 10f;
    
    [Range(0.1f, 50f)]
    public float _maxDrawTime = 2f;
    
    [Range(0.1f, 10f)]
    public float _reloadTime = 1f;

    float _drawTime = 0.0f;
    float _drawFactor = 0.0f;
    float _timeSinceArrowFired = 0.0f;
    enum AnimationState { Idle = 1, Shooting = 2, Drawing = 4}
    GameObject _loadedArrow;
    AnimationState _animationState = AnimationState.Idle;

    private void OnValidate() 
    {
        _maxArrowSpeed = _minArrowSpeed > _maxArrowSpeed ? _minArrowSpeed : _maxArrowSpeed;
    }

    private void Awake() {
        LoadArrow();
        shootNoodleSoundEvent = FMODUnity.RuntimeManager.CreateInstance(shootNoodleEvent);
    }

    float GetArrowSpeed(float minSpeed, float maxSpeed, float draw) {
        draw = Mathf.Clamp(draw, 0, 1);
        return Mathf.Lerp(minSpeed, maxSpeed, draw);
    }

    GameObject SpawnArrow(GameObject arrowSpawn) {
        var arrow = GameObject.Instantiate(_arrow, arrowSpawn.transform.position, arrowSpawn.transform.rotation);
        var arrowSpawnTrans = _arrowSpawn.transform;
        arrow.transform.LookAt(arrowSpawnTrans.position + arrowSpawnTrans.forward, arrowSpawnTrans.up);
        return arrow;
    }

    void LoadArrow()
    {
        _loadedArrow = SpawnArrow(_arrowSpawn);
        _loadedArrow.transform.parent = _arrowSpawn.transform;
        _loadedArrow.GetComponent<Rigidbody>().useGravity = false;
    }

    void FireArrow(float drawFactor, float maxSpeed)
    {
        shootNoodleSoundEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        shootNoodleSoundEvent.setParameterValue("BowChargeRelease", 1);
        var arrowSpawnTrans = _arrowSpawn.transform;

        _loadedArrow.GetComponent<Rigidbody>().useGravity = true;
        _loadedArrow.transform.parent = null;
        _loadedArrow.GetComponent<Rigidbody>().velocity = arrowSpawnTrans.forward * GetArrowSpeed(_minArrowSpeed, _maxArrowSpeed, drawFactor);
        _loadedArrow = null;
    }
    
    void ResetDraw() {
        _drawFactor = 0.0f;
        _drawTime = 0.0f;
    }

    void HandleAnimation(AnimationState state) 
    {
        _animator.speed = 1f;
        switch (state) {
            case AnimationState.Drawing:
                var aimClipLength = 0.23f;
                var drawSpeed =  aimClipLength / _maxDrawTime;
                _animator.speed = drawSpeed;
                _animator.SetBool("Aiming", true);
                
                break;
            case AnimationState.Shooting:
                _animator.SetTrigger("Shooting");
                _animator.SetBool("Aiming", false);
                break;
            case AnimationState.Idle:
                _animator.SetBool("Aiming", false);
                break;
        }
    }

    void Update()
    {
        _timeSinceArrowFired += Time.deltaTime;
        if (Input.GetMouseButtonUp(0) && _loadedArrow != null) 
        {
            FireArrow(_drawFactor, _maxArrowSpeed);
            ResetDraw();
            _animationState = AnimationState.Shooting;
            _timeSinceArrowFired = 0.0f;
        } 
        else if(Input.GetMouseButtonDown(1)) {
            shootNoodleSoundEvent.start();
            shootNoodleSoundEvent.setParameterValue("BowChargeRelease", 0);
        } 
        else if (Input.GetMouseButton(1)) {
            _animationState = AnimationState.Drawing;
            _drawTime = _drawTime < _maxDrawTime ? _drawTime + Time.deltaTime : _maxDrawTime;
            _drawFactor = _drawTime / _maxDrawTime;
        } else if(Input.GetMouseButtonUp(1)) {
            _animationState = AnimationState.Idle;
            shootNoodleSoundEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        if(_loadedArrow == null && _animationState != AnimationState.Shooting && _timeSinceArrowFired > _reloadTime) {
            LoadArrow();
        }

        HandleAnimation(_animationState);
    }
}
