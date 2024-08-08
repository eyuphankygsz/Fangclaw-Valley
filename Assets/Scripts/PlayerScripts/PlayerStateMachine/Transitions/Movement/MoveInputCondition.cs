using Zenject;

public class MoveInputCondition : AbstractCondition
{
	[Inject]
	private InputManager _inputManager;
	public override bool CheckCondition()
	{
		if (_inputManager.Controls.Player.Movement.IsPressed())
			return true;

		return false;
	}
}
