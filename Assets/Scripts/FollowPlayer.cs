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
    public NavMeshAgent nav;
    Rigidbody rgb;
    private Vector3 enemyTransform;
    private Vector3 enemyPosition;
    private FollowPlayer followScript;
    private PlayerController playerController;
    private Animator anim;

    public HealthBar healthBar;
    public int health;
    private bool attackTimer = true;
    private bool attackCooldown;
    private float timer = 3f;

    public AIState currentState;
    private void Start()
    {
        //playerController =  GetComponent<PlayerController>
        nav = GetComponent<NavMeshAgent>();
        rgb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        currentState = AIState.Follow;
        healthBar.SetMaxHealth(health);
        healthBar.SetFillColour(Color.green);

    }

    private void Update()
    {
        switch (currentState)
        {
            default:
            case AIState.Idle:
                transform.rotation = player.transform.rotation;
                //anim.SetBool("IsMoving", false);
                break;
            case AIState.Follow:
                anim.SetBool("IsMoving", true);
                //nav.transform.forward = player.transform.forward;
                transform.rotation = Camera.main.transform.rotation;
                nav.SetDestination(playerTarget.position);
                break;
            case AIState.Pinged:
                anim.SetBool("IsMoving", true);
                pingedTarget = new Vector3(pingedTarget.x, 0, pingedTarget.z);
                nav.SetDestination(pingedTarget);
                break;
            case AIState.Fighting:
                Attack();
                if (nav.remainingDistance <=10f && nav.remainingDistance != 0)
                {
                    nav.isStopped = true;
                }
                else
                {
                    enemyPosition = new Vector3(enemyTransform.x, 0, enemyTransform.z);
                    nav.SetDestination(enemyPosition);
                }
                break;
            case AIState.Dead:

                break;
        }
        
        if(nav.velocity == Vector3.zero)
        {
            anim.SetBool("IsMoving", false);
            //SetState(AIState.Idle);
        }
        Attack();

        if(rgb.velocity != Vector3.zero)
        {
            
        }
        else
        {
            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Enemy")
        {
            enemyTransform = other.transform.position;
            SetState(AIState.Fighting);
            if(attackTimer)
            {
                other.GetComponentInParent<EnemyAttack>().ReduceHealth(1);
                attackCooldown = true;
                attackTimer = false;
            }
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            currentState = AIState.Idle;

        }
    }

    public void SetState(AIState newState)
    {
        currentState = newState;
    }

    private void Attack()
    {
        if (attackCooldown)
        {
            if (timer <= 0)
            {
                timer = 3f;
                attackTimer = true;
                attackCooldown = false;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
    }
    public void ReduceHealth(int value)
    {
        health -= value;
        healthBar.SetHealth(health);
    }
}
