using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Chef : MonoBehaviour
{
    public delegate void ChefDiedDelegate(Chef chef);
    public event ChefDiedDelegate ChefDied;
    
    public ChefManager ChefManager;

    UnityEvent _DeathEvent = new UnityEvent();   
    CrackManager _crackManager;
    Vector3 _HomePos;
    NavMeshAgent _NavAgent;
    CrackBehaviour _Crack;
    GameObject _PatrolWaypoint;
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
    static int _currentPatrolPointIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        _HomePos = transform.position;
        _crackManager = FindObjectOfType<CrackManager>();
        _NavAgent = GetComponent<NavMeshAgent>();
        ChangeState(ChefState.Patrol);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if(OnWall())
        {
            _ChefAnimator.SetBool("Climb",true);
            _MeshTransform.localRotation = Quaternion.Euler(90,0,0);
        }
        else
        {
            _ChefAnimator.SetBool("Climb",false);
            _MeshTransform.localRotation = Quaternion.Euler(0,0,0);
        }
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
                _Crack = GetClosestAvailableCrack();
                if(_Crack != null) {
                    ChangeState(ChefState.GoToTarget);
                }
                if (ReachedTarget(_PatrolWaypoint.transform.position))
                {
                    _PatrolWaypoint = GetNextPatrolTarget();
                    _NavAgent.SetDestination(_PatrolWaypoint.transform.position);
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

    CrackBehaviour GetClosestAvailableCrack() {
        CrackBehaviour closestCrack = null;
        float smallestDist = 1000000;
        foreach(var crack in _crackManager.RepairedCracks) {
            var distToCrack = (transform.position - crack.transform.position).magnitude;

            if(distToCrack < smallestDist && crack.CanTake()) {
                smallestDist = distToCrack;
                closestCrack = crack;
            }
        }
        
        if(closestCrack != null) {
            closestCrack.Take();
        }

        return closestCrack;
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
                _PatrolWaypoint = GetNextPatrolTarget();
                _NavAgent.SetDestination(_PatrolWaypoint.transform.position);
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

    GameObject GetNextPatrolTarget() {
        if(_currentPatrolPointIndex == 0) {
            _currentPatrolPointIndex = 1;
        } else {
            _currentPatrolPointIndex = 0;
        }
        return ChefManager.PatrolPoints[_currentPatrolPointIndex];
    }

    bool ReachedTarget(Vector3 _targetPos)
    {
        float minDistance = 2f;
        float distance = (_targetPos - transform.position).magnitude;
        return minDistance > distance; 
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

    bool OnWall()
    {
       //Debug.Log("x " + transform.rotation.eulerAngles.x);
        float minX = 0.1f;
        return minX < transform.rotation.eulerAngles.x;//minY <transform.forward.y  || -minY > transform.forward.y ;
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
