using UnityEngine;
using Zenject;

public class CrouchInputCondition : AbstractCondition
{
	[Inject]
	private InputManager _inputManager;
	public override bool CheckCondition()
	{
		if (_inputManager.Controls.Player.Crouch.IsPressed())
			return true;

		return false;
	}
}
