using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AIState 
{   Follow, 
    Pinged, 
    Idle, 
    Fighting, 
    Dead }

public class FollowPlayer : MonoBehaviour
{
    public Transform playerTarget;
    public Transform player;
    public Vector3 pingedTarget;
    NavMeshAgent nav;
    Rigidbody rgb;
    private Vector3 enemyTransform;
    private Vector3 enemyPosition;
    private FollowPlayer followScript;
    public int health;
    
    public AIState currentState;
    private void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        rgb = GetComponent<Rigidbody>();

        currentState = AIState.Idle;

    }

    private void Update()
    {
        switch (currentState)
        {
            default:
            case AIState.Idle:
                transform.rotation = player.transform.rotation;
                break;
            case AIState.Follow:
                nav.transform.forward = player.transform.forward;
                nav.SetDestination(playerTarget.position);
                break;
            case AIState.Pinged:
                pingedTarget = new Vector3(pingedTarget.x, 0, pingedTarget.z);
                nav.SetDestination(pingedTarget);
                /*if (nav.remainingDistance <= 1.0f) 
                {
                    nav.isStopped = true;
                    currentState = AIState.Idle;
                }*/
                break;
            case AIState.Fighting:
                enemyPosition = new Vector3(enemyTransform.x, 0, enemyTransform.z);
                nav.SetDestination(enemyPosition);
                if(enemyPosition.x == this.transform.position.x && enemyPosition.z == this.transform.position.z)
                {

                }
                break;
            case AIState.Dead:

                break;
        }

        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            
            enemyTransform = other.transform.position;
            setState(AIState.Fighting); 
            

        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            currentState = AIState.Idle;

        }
    }

    public void setState(AIState newState)
    {
        currentState = newState;
    }
}
