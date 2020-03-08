using UnityEngine;

public class PlayerDuckingState : PlayerBaseState
{
    public override void Enterstate(PlayerController_FSM player)
    {
        player.SetExpression(player.duckingSprite);
        player.head.localPosition = new Vector3(player.head.localPosition.x, 0.5f, player.head.localPosition.z);
    }

    public override void onCollisionEnter(PlayerController_FSM player)
    {
        // nothing to do here
    }

    public override void Update(PlayerController_FSM player)
    {
        if (Input.GetButtonUp("Duck")) {
            player.head.localPosition = new Vector3(player.head.localPosition.x, 0.8f, player.head.localPosition.z);
            player.TransitionToState(player.idleState);
        }
    }
}
