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
    Transform HomTransform;

    NavMeshAgent NavAgent;
    // Start is called before the first frame update
    void Start()
    {
        NavAgent = GetComponent<NavMeshAgent>();
        GoToTarget(TargetTest);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        float minDistance = 0.1f;
        Vector3 deltaPos = TargetTest.position - transform.position;
        if (minDistance * minDistance < deltaPos.sqrMagnitude)
        {
            GoHome();
        }
    }

    void GoToTarget(Transform _target)
    {
        NavAgent.SetDestination(_target.position);
    }

    void GoHome()
    { }



}
