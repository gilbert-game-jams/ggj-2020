using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Chef : MonoBehaviour
{

    [SerializeField]
    Transform TargetTest;
    [SerializeField]
    Transform HomeTransform;
    NavMeshAgent NavAgent;
    CrackBehaviour crack;

    enum ChefState {GoToTarget, GoHome};
    ChefState state = ChefState.GoToTarget;

    // Start is called before the first frame update
    void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
      ChangeState(ChefState.GoToTarget);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        switch(state)
        {
        case  ChefState.GoHome:
         break;
         case ChefState.GoToTarget:
             if(!ReachedTarget(TargetTest.position))
        return;
        crack.UndoRepair();
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
            crack = TargetTest.GetComponent<CrackBehaviour>();
            NavAgent.SetDestination(TargetTest.position);
            state = ChefState.GoToTarget;
            break;
        }
    }


    bool ReachedTarget(Vector3 _targetPos)
    {
        float minDistance = 1f;
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
   }

   void GetNextNoodle()
   {
       crack = TargetTest.gameObject.GetComponent<CrackBehaviour>();
   }

}
