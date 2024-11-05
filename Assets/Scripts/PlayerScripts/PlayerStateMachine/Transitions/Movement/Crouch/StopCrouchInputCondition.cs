using Zenject;

public class StopCrouchInputCondition : AbstractCondition
{
	[Inject]
	private InputManager _inputManager;
	public override bool CheckCondition()
	{
		if (!_inputManager.Controls.Player.Crouch.IsPressed())
			return true;

		return false;
	}

    public override void ResetFrameFreeze()
    {

    }
}
