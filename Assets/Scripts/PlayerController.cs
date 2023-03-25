using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private InputManager inputManager;
    private Transform cameraTransform;
    private Transform playerTransform;

    [SerializeField]
    private float playerSpeed = 2.0f;
    [SerializeField]
    private float jumpHeight = 1.0f;
    [SerializeField]
    private float gravityValue = -9.81f;

    public GameObject squadAI;
    public Object ping;
    private FollowPlayer followScript;
    private Vector3 newPoint;
    private Object pingObj;
    public CinemachineVirtualCamera vcam1;
    public CinemachineVirtualCamera vcam2;
    public GameObject topTrack;
    private GameObject TopDownCam;
    private Vector3 hitPoint;
    private GameObject closestSquad;
    private AIState closestSquadState;
    private bool lockPlayer;
    public GameObject lowestMember;

    public TextMeshProUGUI SquadMemberText;

    public GameObject followSoloPingObj;
    public GameObject followGroupPingObj;
    public GameObject needHelpPingObj;
    public GameObject fallBackSoloPingObj;
    public GameObject fallBackGroupPingObj;
    public GameObject cancelPingObj;
    public GameObject regroupPingObj;

    public GameObject SquadLowPromptObj;
    public GameObject SquadMemberLowPromptObj;
    public GameObject SquadMemberFarAwayObj;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
        playerTransform = GetComponent<Transform>();
        followScript = squadAI.GetComponent<FollowPlayer>();
    }

    void Update()
    {
        if(inputManager.PlayerSwapCam())
        {
            if(vcam1.Priority > vcam2.Priority)
            {
                topTrack.transform.position = new Vector3(this.transform.position.x, topTrack.transform.position.y, this.transform.position.z);
                vcam2.Priority = 11;
            }
            else
            {
                vcam2.Priority = 9;
            }
        }
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector2 movement = inputManager.GetPlayerMovement();
        if(!lockPlayer)
        {
            if (vcam2.Priority < vcam1.Priority)
            {

                Vector3 move = new Vector3(movement.x, 0f, movement.y);
                move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
                move.y = 0f;
                controller.Move(move * Time.deltaTime * playerSpeed);
                playerTransform.forward = new Vector3(cameraTransform.forward.x, playerTransform.forward.y, playerTransform.forward.z);

                if (inputManager.PlayerJumpedThisFrame() && groundedPlayer)
                {
                    playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
                }

                playerVelocity.y += gravityValue * Time.deltaTime;
                controller.Move(playerVelocity * Time.deltaTime);
            }
            else
            {
                Vector3 move = new Vector3(movement.x, 0f, movement.y);
                move = topTrack.transform.forward * move.z + topTrack.transform.right * move.x;
                move.y = 0f;
                topTrack.transform.Translate(move * Time.deltaTime * 30.0f);

            }
        }
            
        if(inputManager.PingWheelPressed())
        {
            PingWheelPressed();
        }

        if (inputManager.PlayerPinged())
        {
            MovePinged(CastRay());
        }
        if (inputManager.PlayerReturn())
        {
            followScript.setState(AIState.Follow);
        }
        if(inputManager.EnterPressed())
        {
            CheckActivePrompt();
        }
    }

    private void SpawnPing(Vector3 point)
    {
        newPoint = new Vector3(point.x, point.y + 2, point.z); 
        pingObj = Object.Instantiate(ping, newPoint, Quaternion.identity);
        Destroy(pingObj, 1.5f);
       
    }

    public void MovePinged(Vector3 coords)
    {
        closestSquad = FindClosestSquadMember();
        closestSquad.GetComponent<FollowPlayer>().pingedTarget = coords;
        closestSquadState = closestSquad.GetComponent<FollowPlayer>().currentState;
        closestSquad.GetComponent<FollowPlayer>().setState(AIState.Pinged);

        SpawnPing(coords);
    }
    private void PingWheelPressed()
    {
        Cancel();
    }

     void wheelEnabled()
    {
        PingWheelScript.wheelEnabledStatic();
        vcam2.enabled = false;
        vcam1.enabled = false;
    }
    
    void wheelDisabled()
    {
        PingWheelScript.wheelDisabledStatic();
        vcam1.enabled = true;
        vcam2.enabled = true;
    }
    public GameObject FindClosestSquadMember()
    {
        
        GameObject[] squadMembers;
        squadMembers = GameObject.FindGameObjectsWithTag("Squad");
        GameObject closest = null;
        float distance = 3000;
        //Vector3 position = transform.position;
        foreach (GameObject gamObj in squadMembers)
        {
            if (vcam1.Priority > vcam2.Priority)
            {
                Vector3 diff = gamObj.transform.position - transform.position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = gamObj;
                    distance = curDistance;
                }
            }
            else
            {
                Vector3 diff = gamObj.transform.position - topTrack.transform.position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = gamObj;
                    distance = curDistance;
                }
            }
        }
        return closest;
    }
    public void FollowPingGroup(Vector3 coords)
    {
        GameObject[] squadMembers;
        squadMembers = GameObject.FindGameObjectsWithTag("Squad");
        foreach (GameObject gamObj in squadMembers)
        {

            gamObj.GetComponent<FollowPlayer>().pingedTarget = coords;
            gamObj.GetComponent<FollowPlayer>().setState(AIState.Pinged);


        }
        SpawnPing(coords);
    }

    public void NeedHelp(Vector3 coords)
    {
        GameObject[] squadMembers;
        squadMembers = GameObject.FindGameObjectsWithTag("Squad");
        GameObject closest = null;
        float distance = 3000;

        foreach (GameObject gamObj in squadMembers)
        {
            if (vcam1.Priority > vcam2.Priority)
            {
                Vector3 diff = gamObj.transform.position - transform.position;
                float curDistance = diff.sqrMagnitude;


                if (curDistance < distance)
                {
                    closest = gamObj;
                    distance = curDistance;
                }
            }
            else
            {
                Vector3 diff = gamObj.transform.position - topTrack.transform.position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    closest = gamObj;
                    distance = curDistance;
                }
            }
        }

        foreach (GameObject gamObj in squadMembers)
        {

            if (gamObj != closest)
            {

                gamObj.GetComponent<FollowPlayer>().pingedTarget = closest.transform.position;
                gamObj.GetComponent<FollowPlayer>().setState(AIState.Pinged);

                SpawnPing(coords);
            }

        }
            
    }
    public void Regroup()
    {
        GameObject[] squadMembers;
        squadMembers = GameObject.FindGameObjectsWithTag("Squad");
        foreach (GameObject gamObj in squadMembers)
        {
            gamObj.GetComponent<FollowPlayer>().setState(AIState.Follow);
        }
    }
    public void FallBackSolo(Vector3 coords)
    {
        closestSquad = FindClosestSquadMember();

        Vector3 direction = closestSquad.transform.position - coords;
        direction = closestSquad.transform.position + (direction * 2f);
        closestSquad.GetComponent<FollowPlayer>().pingedTarget = direction;
        closestSquad.GetComponent<FollowPlayer>().setState(AIState.Pinged);

        SpawnPing(coords);
    }
    public void FallBackGroup(Vector3 coords)
    {
        GameObject[] squadMembers;
        squadMembers = GameObject.FindGameObjectsWithTag("Squad");
        closestSquad = FindClosestSquadMember();

        foreach (GameObject gamObj in squadMembers)
        {
            Vector3 direction = gamObj.transform.position - coords;
            direction = closestSquad.transform.position + (direction * 2f);
            gamObj.GetComponent<FollowPlayer>().pingedTarget = direction;
            gamObj.GetComponent<FollowPlayer>().setState(AIState.Pinged);
        }

        SpawnPing(coords);
    }
    public void Cancel()
    {
        if (!PingWheelScript.IsVisibleStatic())
        {
            closestSquad = FindClosestSquadMember();
            closestSquadState = closestSquad.GetComponent<FollowPlayer>().currentState;
            SquadMemberText.gameObject.GetComponent<TextMeshProUGUI>().text = "Closest Member: " + closestSquad.name;
            if (closestSquadState == AIState.Follow)
            {
                followSoloPingObj.SetActive(true);
                followGroupPingObj.SetActive(true);
                needHelpPingObj.SetActive(false);
                fallBackSoloPingObj.SetActive(false);
                fallBackGroupPingObj.SetActive(false);
                cancelPingObj.SetActive(true);
                regroupPingObj.SetActive(false);
            }
            if (closestSquadState == AIState.Pinged)
            {
                followSoloPingObj.SetActive(true);
                followGroupPingObj.SetActive(true);
                needHelpPingObj.SetActive(true);
                fallBackSoloPingObj.SetActive(false);
                fallBackGroupPingObj.SetActive(false);
                cancelPingObj.SetActive(true);
                regroupPingObj.SetActive(true);
            }
            if (closestSquadState == AIState.Idle)
            {
                followSoloPingObj.SetActive(true);
                followGroupPingObj.SetActive(true);
                needHelpPingObj.SetActive(true);
                fallBackSoloPingObj.SetActive(true);
                fallBackGroupPingObj.SetActive(true);
                cancelPingObj.SetActive(true);
                regroupPingObj.SetActive(true);
            }
            if (closestSquadState == AIState.Fighting)
            {
                followSoloPingObj.SetActive(false);
                followGroupPingObj.SetActive(false);
                needHelpPingObj.SetActive(true);
                fallBackSoloPingObj.SetActive(true);
                fallBackGroupPingObj.SetActive(true);
                cancelPingObj.SetActive(true);
                regroupPingObj.SetActive(false);
            }
            
            wheelEnabled();
            lockPlayer = true;
        }
        else
        {
            wheelDisabled();
            lockPlayer = false;
        }
    }
    void CheckActivePrompt()
    {
        if(SquadLowPromptObj.activeSelf)
        {
            Regroup();
            SquadLowPromptObj.SetActive(false);
        }
        else if (SquadMemberLowPromptObj.activeSelf)
        {
            GameObject[] squadMembers;
            squadMembers = GameObject.FindGameObjectsWithTag("Squad");
            int lowHealth = 10;
            
            foreach (GameObject gamObj in squadMembers)
            {
                if(gamObj.GetComponent<FollowPlayer>().health < lowHealth)
                {
                    lowHealth = gamObj.GetComponent<FollowPlayer>().health;
                    lowestMember = gamObj;
                }
            }
            NeedHelp(lowestMember.transform.position);
            SquadMemberLowPromptObj.SetActive(false);
        }
        else if (SquadMemberFarAwayObj.activeSelf)
        {
            GameObject[] squadMembers;
            squadMembers = GameObject.FindGameObjectsWithTag("Squad");
            foreach(GameObject gamObj in squadMembers)
            {
                Vector3 diff = gamObj.transform.position - transform.position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance >= 6000)
                {
                    gamObj.GetComponent<FollowPlayer>().setState(AIState.Follow);
                }
            }
            SquadMemberFarAwayObj.SetActive(false);
        }
    }

    public Vector3 CastRay()
    {
        if(Physics.Raycast(cameraTransform.position, cameraTransform.TransformDirection(Vector3.forward), out RaycastHit hit, 80f))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}
