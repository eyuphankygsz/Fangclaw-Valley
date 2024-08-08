public class StandingCondition : AbstractCondition
{
	[UnityEngine.SerializeField]
	private PlayerCrouchState _crouchState;
	public override bool CheckCondition()
	{
		if (!_crouchState.Crouched && !_crouchState.Crouching)
			return true;

		return false;
	}
}
