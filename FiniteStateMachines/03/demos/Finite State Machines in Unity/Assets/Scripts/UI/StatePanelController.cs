using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StatePanelController : MonoBehaviour
{

	public Image IdleImage;
	public Image JumpingImage;
	public Image DuckingImage;
	public Image SpinningImage;


	public Color PressedColor;
	public Color ReleasedColor;

	private PlayerController_FSM player;
	
	// Use this for initialization
	void Start ()
	{
		player = FindObjectOfType<PlayerController_FSM>();
	}

    // Update is called once per frame
    void Update()
    {
        if (null != player && null != player.CurrentState)
            ShowState(player.CurrentState);
    }

    void ShowState(PlayerBaseState state)
    {
        if (state.GetType() == typeof(PlayerIdleState))
        {
            setButtonDown(IdleImage);
            setButtonUp(JumpingImage);
            setButtonUp(DuckingImage);
            setButtonUp(SpinningImage);
        }
        else if (state.GetType() == typeof(PlayerJumpingState))
        {
            setButtonUp(IdleImage);
            setButtonDown(JumpingImage);
            setButtonUp(DuckingImage);
            setButtonUp(SpinningImage);
        }
        else if (state.GetType() == typeof(PlayerDuckingState))
        {
            setButtonUp(IdleImage);
            setButtonUp(JumpingImage);
            setButtonDown(DuckingImage);
            setButtonUp(SpinningImage);
        }
        else if (state.GetType() == typeof(PlayerSpinningState))
        {
            setButtonUp(IdleImage);
            setButtonUp(JumpingImage);
            setButtonUp(DuckingImage);
            setButtonDown(SpinningImage);
        }
    }

    private void setButtonDown(Image button)
	{
		button.color = PressedColor;
		button.GetComponentInChildren<Text>().color = Color.white;
	}
	
	private void setButtonUp(Image button)
	{
		button.color = ReleasedColor;
		button.GetComponentInChildren<Text>().color = Color.black;
	}
}
