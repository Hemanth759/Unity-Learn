using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpingState : PlayerBaseState
{
    public override void Enterstate(PlayerController_FSM player)
    {
        player.SetExpression(player.jumpingSprite);
    }

    public override void onCollisionEnter(PlayerController_FSM player)
    {
        player.TransitionToState(player.idleState);
    }

    public override void Update(PlayerController_FSM player)
    {
        if (Input.GetButtonDown("Duck")) {
            player.TransitionToState(player.spinningState);
        }
    }
}
