using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerValues : MonoBehaviour
{
    private GameObject[] squadMembers;
    public int currentHealth;
    private bool healthPromptShown;
    private bool memberLowPromptShown;
    private bool memberFarAwayPromptShown;
    public GameObject SquadLowPromptObj;
    public GameObject SquadMemberLowPromptObj;
    public GameObject SquadMemberFarAwayPromptObj;

    private void Awake()
    {
        squadMembers = GameObject.FindGameObjectsWithTag("Squad");
        SquadLowPromptObj.SetActive(false);
        SquadMemberLowPromptObj.SetActive(false);
        SquadMemberFarAwayPromptObj.SetActive(false);
        healthPromptShown = false;
        memberLowPromptShown = false;
        memberFarAwayPromptShown = false;
    }

    private void Update()
    {
        if (CurrentSquadHealth() <= 15 && !healthPromptShown)
        {
            SquadMemberLowPromptObj.SetActive(false);
            SquadMemberFarAwayPromptObj.SetActive(false);
            SquadLowPromptObj.SetActive(true);
            StartCoroutine(PromptTimerSquadLow());
            
        }
        CurrentSquadDistance();


    }

    int CurrentSquadHealth()
    {
        currentHealth = 0;
        foreach (GameObject gamObj in squadMembers)
        {
            if (gamObj.GetComponent<FollowPlayer>().currentState != AIState.Dead)
            {
                currentHealth += gamObj.GetComponent<FollowPlayer>().health;
                if(gamObj.GetComponent<FollowPlayer>().health <= 3 && !memberLowPromptShown)
                {
                    SquadMemberLow(gamObj);
                }
            }
        }
        return currentHealth;
    }
    void CurrentSquadDistance()
    {
        foreach (GameObject gamObj in squadMembers)
        {
            if (gamObj.GetComponent<FollowPlayer>().currentState != AIState.Dead)
            {
                Vector3 diff = gamObj.transform.position - transform.position;
                float curDistance = diff.sqrMagnitude;
                if(curDistance >= 6000 && !SquadMemberLowPromptObj.activeSelf && !SquadLowPromptObj.activeSelf)
                {
                    SquadMemberFarAway();
                }
            }
        }
    }
    void SquadMemberLow(GameObject obj)
    {
        if(!SquadLowPromptObj.activeSelf)
        {
            SquadMemberLowPromptObj.SetActive(true);
            StartCoroutine(PromptTimerMemberLow());
        }
    }
    void SquadMemberFarAway()
    {
        if(!memberFarAwayPromptShown)
        {
            SquadMemberFarAwayPromptObj.SetActive(true);
            StartCoroutine(PromptTimerMemberFarAway());
        }
        

    }
    private IEnumerator PromptTimerSquadLow()
    {
        healthPromptShown = true;
        yield return new WaitForSeconds(30);
        
        if(SquadLowPromptObj.activeSelf)
        {
            SquadLowPromptObj.SetActive(false);
        }
        
    }
    private IEnumerator PromptTimerMemberLow()
    {
        memberLowPromptShown = true;
        yield return new WaitForSeconds(30);

        if (SquadMemberLowPromptObj.activeSelf)
        {
            SquadMemberLowPromptObj.SetActive(false);
        }

    }
    private IEnumerator PromptTimerMemberFarAway()
    {
        memberFarAwayPromptShown = true;
        yield return new WaitForSeconds(30);

        if (SquadMemberFarAwayPromptObj.activeSelf)
        {
            SquadMemberFarAwayPromptObj.SetActive(false);
        }

    }

}
