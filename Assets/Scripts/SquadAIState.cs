using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadAIState : MonoBehaviour
{
    [SerializeField]
    enum AIState { Follow, Pinged, Idle}

    private void Awake()
    {
        AIState state;
        state = AIState.Follow;
    }

}
