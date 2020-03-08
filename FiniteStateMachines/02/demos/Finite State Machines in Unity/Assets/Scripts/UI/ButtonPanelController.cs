using UnityEngine.UI;
using UnityEngine;

public class ButtonPanelController : MonoBehaviour
{

	public Image JumpButton;
	public Image DuckButton;
	public Image SwapButton;

	public Color PressedColor;
	public Color ReleasedColor;

	// Use this for initialization
	void Start () 
	{
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetButtonDown("Jump"))
			setButtonDown(JumpButton);
		
		if (Input.GetButtonUp("Jump"))
			setButtonUp(JumpButton);
		
		if (Input.GetButtonDown("Duck"))
			setButtonDown(DuckButton);
		
		if (Input.GetButtonUp("Duck"))
			setButtonUp(DuckButton);
		
		if (Input.GetButtonDown("SwapWeapon"))
			setButtonDown(SwapButton);
		
		if (Input.GetButtonUp("SwapWeapon"))
			setButtonUp(SwapButton);
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
