public class StopRunStaminaCondition : AbstractCondition
{
	[UnityEngine.SerializeField]
	private PlayerStamina _playerStamina;
	public override bool CheckCondition()
	{
		if (_playerStamina.Stamina == 0)
			return true;

		return false;
	}
}
