
using UnityEngine;

public class RunStaminaCondition : AbstractCondition
{
	[UnityEngine.SerializeField]
	private PlayerStamina _playerStamina;

	[UnityEngine.SerializeField]
	private int _minStamina;

	public override bool CheckCondition()
	{
		Debug.Log(_playerStamina.Stamina + "  " + _minStamina);
		if (_playerStamina.Stamina > _minStamina)
			return true;

		return false;
	}
}
