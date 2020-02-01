using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(NavMeshAgent))]
public class Chef : MonoBehaviour
{
    UnityEvent DeathEvent = new UnityEvent();

    CrackManager crackManager;
    Vector3 HomePos;
    NavMeshAgent NavAgent;
    CrackBehaviour Crack;
    Vector3 PatrolWaypoint = Vector3.zero;
    enum ChefState {GoToTarget, GoHome,Patrol, Eat};
    ChefState state = ChefState.GoToTarget;

    [SerializeField]
    float TimeToEat = 5;
    float EatTimer;

    // Start is called before the first frame update
    void Start()
    {
        HomePos = transform.position;
        crackManager = FindObjectOfType<CrackManager>();
      NavAgent = GetComponent<NavMeshAgent>();
      ChangeState(ChefState.Patrol);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        switch(state)
        {
        case  ChefState.GoHome:
         break;
         case ChefState.GoToTarget:
        if(Crack == null)
        {
            ChangeState(ChefState.Patrol);
            return;
        }
        if(!ReachedTarget(Crack.transform.position))
        return;
           ChangeState(ChefState.Eat);
         break;
           
           case ChefState.Patrol:
            
             if(ReachedTarget(PatrolWaypoint))
             {
                if(crackManager.TryGetCrack(out Crack))
                {
                    if(Crack != null)
                    {
                        Crack.Take();
                        NavAgent.SetDestination(Crack.transform.position);
                        ChangeState(ChefState.GoToTarget);
                        return;
                    }
                }
                PatrolWaypoint = crackManager.GetPatrolPoint();
                NavAgent.SetDestination(PatrolWaypoint);
             }
           break;
            case ChefState.Eat:

            if(!Crack.gameObject.activeSelf)
            {
                NavAgent.isStopped = false;
                ChangeState(ChefState.Patrol);
                return;
            }

            if(EatTimer < Time.time)
            {
                Crack.UndoRepair();
                NavAgent.isStopped = false;
                ChangeState(ChefState.Patrol);
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
            state = ChefState.GoHome;
            break;
            case ChefState.GoToTarget:
            NavAgent.SetDestination(Crack.transform.position);
            state = ChefState.GoToTarget;
            break;
            case ChefState.Patrol:
            PatrolWaypoint = crackManager.GetPatrolPoint();
            NavAgent.SetDestination(PatrolWaypoint);
            state = ChefState.Patrol;
            break;
            case ChefState.Eat:
            NavAgent.isStopped = true;
            EatTimer = Time.time  + TimeToEat;
            state = ChefState.Eat;
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
        NavAgent.SetDestination(_target.position);
    }

    void GoHome()
    {
        Debug.Log("Go Home");
        NavAgent.SetDestination(HomePos);
    }

   private void OnTriggerEnter(Collider other) 
   {
       if(other.GetComponent<ArrowBehaviour>() != null)
       {
           if(DeathEvent.GetPersistentEventCount() <= 0)
           return;
           DeathEvent.Invoke();
           Destroy(this);
       }
   }


   void SubscribeToDeathEventChef(ChefManager _manager)
   {
       DeathEvent.AddListener(_manager.RemoveChef);
   }

    private void OnDisable()
    {
        DeathEvent.RemoveAllListeners();
    }

}
