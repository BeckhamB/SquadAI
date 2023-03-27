using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int health = 10;
    public HealthBar healthBar;
    private bool attackTimer = true;
    private bool attackCooldown;
    private float timer = 3f;

    private void Start()
    {
        healthBar.SetMaxHealth(health);
        healthBar.SetFillColour(Color.red);
    }
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Trigger")
        {
            if (attackTimer)
            {
                other.GetComponentInParent<FollowPlayer>().ReduceHealth(1);
                attackCooldown = true;
                attackTimer = false;
            }
        }
    }
    private void Update()
    {
        Attack();
        if(health == 0)
        {
            this.transform.position = new Vector3(1000, 0 , 1000);
            //Destroy(this.gameObject);
        }

    }
    private void Attack()
    {
        if(attackCooldown)
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
