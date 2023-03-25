using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : MonoBehaviour
{
    private Rigidbody rb;
    public Vector3 walkPoint;
    public NavMeshAgent agent;
    bool walkPointSet;
    public float walkPointRange;

    private void Start()
    {
        //rb = GetComponent<Rigidbody>();
        walkPoint = new Vector3(10, 0, 0);
    }
    // Update is called once per frame
    void Update()
    {
        Patrolling();
    }
    
    private void Patrolling()
    {
        if (!walkPointSet)
        {
            SearchWalkPoint();
        }

        if(walkPointSet)
        {
            agent.SetDestination(walkPoint);
        }
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if(distanceToWalkPoint.magnitude < 1f)
        {
            walkPointSet = false;
        }
    }
    private void SearchWalkPoint()
    {
        walkPoint = new Vector3(-walkPoint.x, transform.position.y, -walkPoint.z);
        walkPointSet = true;
    }

}
