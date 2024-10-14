using UnityEngine;
using Zenject;

public class JumpInputCondition : AbstractCondition
{
	[Inject]
	private InputManager _inputManager;
	public override bool CheckCondition()
	{
		if (_inputManager.Controls.Player.Jump.IsPressed())
			return true;
		return false;
	}
}
