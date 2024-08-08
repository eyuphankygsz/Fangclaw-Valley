using Zenject;

public class RunInputCondition : AbstractCondition
{
	[Inject]
	private InputManager _inputManager;
	public override bool CheckCondition()
	{
		if (_inputManager.Controls.Player.Run.IsPressed())
			return true;

		return false;
	}
}
