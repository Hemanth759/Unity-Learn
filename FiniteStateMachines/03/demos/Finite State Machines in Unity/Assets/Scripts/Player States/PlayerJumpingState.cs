using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    public override void EnterState(PlayerController_FSM player)
    {
        player.SetExpression(player.jumpingSprite);
    }

    public override void OnCollisionEnter(PlayerController_FSM player)
    {
        player.TransitionToState(player.IdleState);
    }

    public override void Update(PlayerController_FSM player)
    {
        if (Input.GetButtonDown("Duck"))
        {
            player.TransitionToState(new PlayerSpinningState());
        }
    }
}
