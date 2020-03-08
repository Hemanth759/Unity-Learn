using UnityEngine;

public abstract class PlayerBaseState
{
    public abstract void Enterstate(PlayerController_FSM player);

    public abstract void Update(PlayerController_FSM player);

    public abstract void onCollisionEnter(PlayerController_FSM player);
}
