using UnityEngine;

public abstract class PlayerBaseState
{
    public abstract void EnterState(PlayerController_FSM player);

    public abstract void Update(PlayerController_FSM player);

    public abstract void OnCollisionEnter(PlayerController_FSM player);
}
