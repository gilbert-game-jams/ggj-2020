using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Chef : MonoBehaviour
{
    CrackManager crackManager;
    [SerializeField]
    Transform HomeTransform;
    NavMeshAgent NavAgent;
    CrackBehaviour Crack;

    enum ChefState {GoToTarget, GoHome};
    ChefState state = ChefState.GoToTarget;

    // Start is called before the first frame update
    void Start()
    {
        crackManager = FindObjectOfType<CrackManager>();
        NavAgent = GetComponent<NavMeshAgent>();
      ChangeState(ChefState.GoToTarget);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        
        switch(state)
        {
        case  ChefState.GoHome:
        if(!ReachedTarget(HomeTransform.position))
        return;
        ChangeState(ChefState.GoToTarget);
         break;

         case ChefState.GoToTarget:
        if(Crack == null)
        {
            if(!crackManager.TryGetCrack(out Crack))
            {
                NavAgent.isStopped = true;
                 return;
            }
            NavAgent.isStopped = false;
            NavAgent.SetDestination(Crack.transform.position);
        
        }
        if(!ReachedTarget(Crack.transform.position))
        return;
        Crack.UndoRepair();
           ChangeState(ChefState.GoHome);
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
            Crack = null;
            state = ChefState.GoToTarget;
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
        NavAgent.SetDestination(HomeTransform.position);
    }

   private void OnTriggerEnter(Collider other) 
   {
       if(other.GetComponent<ArrowBehaviour>() != null)
       {
           Destroy(this);
       }
   }



}
