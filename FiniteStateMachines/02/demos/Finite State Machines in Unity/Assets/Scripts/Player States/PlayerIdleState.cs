using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public override void Enterstate(PlayerController_FSM player)
    {
        player.SetExpression(player.idleSprite);
    }

    public override void onCollisionEnter(PlayerController_FSM player)
    {
        // nothing to do here
    }

    public override void Update(PlayerController_FSM player)
    {
        if (Input.GetButtonDown("Jump")) {
            player.Rbody.AddForce(Vector3.up * player.jumpForce);
            player.TransitionToState(player.jumpingState);
        } else if (Input.GetButtonDown("Duck")) {
            player.TransitionToState(player.duckingState);
        } else if (Input.GetButtonDown("SwapWeapon"))
        {
            bool usingWeapon01 = player.weapon01.gameObject.activeInHierarchy;

            player.weapon01.gameObject.SetActive(usingWeapon01 == false);
            player.weapon02.gameObject.SetActive(usingWeapon01);
        }
    }
}
