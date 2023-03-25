using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PingWheelScript : MonoBehaviour
{
    private static PingWheelScript instance;
    public InputManager inputManager;
    public PlayerController playerController;
    public GameObject uiCanvas;
    GraphicRaycaster uiRaycaster;

    PointerEventData clickData;
    List<RaycastResult> clickResults;

    private void Awake()
    {
        uiRaycaster = uiCanvas.GetComponent<GraphicRaycaster>();
        clickData = new PointerEventData(EventSystem.current);
        clickResults = new List<RaycastResult>();

        instance = this;
        wheelDisabled();
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void Update()
    {
        IsMouseOverUI();
        if(inputManager.MouseClicked())
        {
            if (IsMouseOverUI())
            {
                GetClicked();
                
            }
        }
        
    }

    private void GetClicked()
    {
        clickData.position = Mouse.current.position.ReadValue();
        clickResults.Clear();

        uiRaycaster.Raycast(clickData, clickResults);
        
        foreach (RaycastResult result in clickResults)
        {
            GameObject uiElement = result.gameObject;
            //Debug.Log(result.gameObject.transform.name);
            if(result.gameObject.transform.name == "FollowSoloPing")
            {
                playerController.MovePinged(playerController.CastRay());
                
            }
            if (result.gameObject.transform.name == "FollowGroupPing")
            {
                playerController.FollowPingGroup(playerController.CastRay());
                Debug.Log("2");
            }
            if (result.gameObject.transform.name == "NeedHelpPing")
            {
                playerController.NeedHelp(playerController.CastRay());
                Debug.Log("3");
            }
            if (result.gameObject.transform.name == "FallBackSoloPing")
            {
                playerController.FallBackSolo(playerController.CastRay());
                Debug.Log("4");
            }
            if (result.gameObject.transform.name == "FallBackGroupPing")
            {
                playerController.FallBackGroup(playerController.CastRay());
                Debug.Log("5");
            }
            if (result.gameObject.transform.name == "CancelPing")
            {
                playerController.Cancel() ;
                Debug.Log("6");
                
            }
            if (result.gameObject.transform.name == "RegroupPing")
            {
                playerController.Regroup();
                Debug.Log("7");
            }
        }
    }

    private void wheelEnabled()
    {
        gameObject.SetActive(true);
    }

    private void wheelDisabled()
    {
        gameObject.SetActive(false);
    }   
    
    public static void wheelEnabledStatic()
    {
        instance.wheelEnabled();
    }

    public static void wheelDisabledStatic()
    {
        instance.wheelDisabled();
    }

    public static bool IsVisibleStatic()
    {
        return instance.gameObject.activeSelf; 
    }
}

