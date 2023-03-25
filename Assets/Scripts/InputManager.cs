using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private PlayerControls playerControls;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        playerControls = new PlayerControls();

    }
    private void OnEnable()
    {
        playerControls.Enable();
    } 

    private void OnDisable()
    {
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return playerControls.Player.Movement.ReadValue<Vector2>();
    }

    public Vector2 GetMouseDelta()
    {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }

    public bool PlayerJumpedThisFrame()
    {
        return playerControls.Player.Jump.triggered;
    }
    
    public bool PlayerPinged()
    {
        return playerControls.Player.Ping.triggered;
    }
    
    public bool PlayerReturn()
    {
        return playerControls.Player.Return.triggered;
    }

    public bool PlayerSwapCam()
    {
        return playerControls.Player.SwapCamera.triggered;
    }

    public bool PingWheelPressed()
    {
        return playerControls.Player.PingWheel.triggered;
    }

    public bool MouseClicked()
    {
        return playerControls.Player.Click.triggered;
    }
    
    public bool EnterPressed()
    {
        return playerControls.Player.Enter.triggered;
    }
}
