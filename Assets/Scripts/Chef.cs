using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Chef : MonoBehaviour
{
    public delegate void ChefDiedDelegate(Chef chef);
    public event ChefDiedDelegate ChefDied;

    UnityEvent _DeathEvent = new UnityEvent();   
    CrackManager _crackManager;
    Vector3 _HomePos;
    NavMeshAgent _NavAgent;
    CrackBehaviour _Crack;
    Vector3 _PatrolWaypoint = Vector3.zero;
    enum ChefState {GoToTarget, GoHome,Patrol, Eat, Dead};
    ChefState _state = ChefState.GoToTarget;

    [SerializeField]
    float _TimeToEat = 5;
    float _EatTimer;
    [SerializeField]
    Animator _ChefAnimator;
    [SerializeField]
    Transform _MeshTransform;
    float _timeUntilDead = 1.0f;
    float _prevZPos;
    // Start is called before the first frame update
    void Start()
    {
        _prevZPos = transform.position.z;
        _HomePos = transform.position;
        _crackManager = FindObjectOfType<CrackManager>();
        _NavAgent = GetComponent<NavMeshAgent>();
        ChangeState(ChefState.Patrol);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        float angle = Vector3.Angle(transform.position + Vector3.right, transform.forward);
       /* if(false)
        {
            Debug.Log("TryToChagne");
            if(MeshTransform.localRotation.x < 90)
            {
                Debug.Log("StartClimb");
            MeshTransform.localRotation = Quaternion.Euler(90f,transform.rotation.y,transform.rotation.z);
            ChefAnimator.SetBool("Climb",true);
            }
        }
        else
        {
             if(MeshTransform.localRotation.x >= 90)
            {
            MeshTransform.localRotation = Quaternion.Euler(0,transform.rotation.y,transform.rotation.z);
            ChefAnimator.SetBool("Climb",false);
            }
        }*/

        switch(_state)
        {
            case ChefState.GoHome:
                break;

            case ChefState.GoToTarget:
                if(_Crack == null)
                {
                    ChangeState(ChefState.Patrol);
                    return;
                }
                if(!ReachedTarget(_Crack.transform.position))
                    return;
                    ChangeState(ChefState.Eat);
                    break;
                
            case ChefState.Patrol:
                    
                if(ReachedTarget(_PatrolWaypoint))
                {
                    if(_crackManager.TryGetCrack(out _Crack))
                    {
                        if(_Crack != null)
                        {
                            _Crack.Take();
                            _NavAgent.SetDestination(_Crack.transform.position);
                    ChangeState(ChefState.GoToTarget);
                    return;
                    }
                }
                _PatrolWaypoint = _crackManager.GetPatrolPoint();
                _NavAgent.SetDestination(_PatrolWaypoint);
                }
                break;

            case ChefState.Eat:

                if(!_Crack.gameObject.activeSelf)
                {
                    _NavAgent.isStopped = false;
                    ChangeState(ChefState.Patrol);
                    return;
                }

                if(_EatTimer < Time.time)
                {
                    _Crack.UndoRepair();
                    _NavAgent.isStopped = false;
                    ChangeState(ChefState.Patrol);
                } 
                break;
            case ChefState.Dead:
                _timeUntilDead -= Time.deltaTime;
                if(_timeUntilDead < 0) {
                    ChefDied.Invoke(this);
                    Destroy(this.gameObject);
                }
                break;
        }
    }


    void ChangeState(ChefState _newState)
    {
        switch(_newState)
        {
            case ChefState.GoHome:
            GoHome();
            _state = ChefState.GoHome;
            break;
            case ChefState.GoToTarget:
            _NavAgent.SetDestination(_Crack.transform.position);
            _state = ChefState.GoToTarget;
            break;
            case ChefState.Patrol:
            _PatrolWaypoint = _crackManager.GetPatrolPoint();
            _NavAgent.SetDestination(_PatrolWaypoint);
            _state = ChefState.Patrol;
            break;
            case ChefState.Eat:
            _ChefAnimator.SetBool("Stealing",true);
            _NavAgent.isStopped = true;
            _EatTimer = Time.time  + _TimeToEat;
            _state = ChefState.Eat;
            break;
        }
    }


    bool ReachedTarget(Vector3 _targetPos)
    {
        float minDistance = 1.5f;
        Vector3 deltaPos = _targetPos - transform.position;
        return minDistance*minDistance > deltaPos.sqrMagnitude; 
    }

    void GoToTarget(Transform _target)
    {
        _NavAgent.SetDestination(_target.position);
    }

    void GoHome()
    {
        Debug.Log("Go Home");
        _NavAgent.SetDestination(_HomePos);
    }

   private void OnTriggerEnter(Collider other) 
   {
       if(other.GetComponent<ArrowBehaviour>() != null)
       {
           _ChefAnimator.SetTrigger("Hit");
           _state = ChefState.Dead;
           _NavAgent.isStopped = true;
       }
   }
}
