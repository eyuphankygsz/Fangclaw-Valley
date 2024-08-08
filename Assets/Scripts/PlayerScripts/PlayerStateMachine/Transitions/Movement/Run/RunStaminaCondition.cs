public class RunStaminaCondition : AbstractCondition
{
	[UnityEngine.SerializeField]
	private PlayerStamina _playerStamina;
	public override bool CheckCondition()
	{
		if (_playerStamina.Stamina > 0.6f)
			return true;

		return false;
	}
}
