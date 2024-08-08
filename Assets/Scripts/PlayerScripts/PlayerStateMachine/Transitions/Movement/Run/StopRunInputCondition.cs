using Zenject;

public class StopRunInputCondition : AbstractCondition
{
	[Inject]
	private InputManager _inputManager;
	public override bool CheckCondition()
	{
		if (!_inputManager.Controls.Player.Run.IsPressed())
			return true;

		return false;
	}
}
