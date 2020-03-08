using UnityEngine;

public class PlayerSpinningState : PlayerBaseState
{
    private float rotation;

    public override void Enterstate(PlayerController_FSM player)
    {
        player.SetExpression(player.spinningSprite);
    }

    public override void onCollisionEnter(PlayerController_FSM player)
    {
        player.transform.rotation = Quaternion.identity;
        player.TransitionToState(player.idleState);
    }

    public override void Update(PlayerController_FSM player)
    {
        float amountToRotate = 900 * Time.deltaTime;
        rotation += amountToRotate;

        if (rotation >= 360) {
            player.transform.rotation = Quaternion.identity;
            rotation = 0;
            player.TransitionToState(player.jumpingState);
        } else {
            player.transform.Rotate(Vector3.up, amountToRotate);
        }
    }
}
