using UnityEngine;

public class StaminaPotion : UseFunction
{
	[SerializeField]
	private PlayerStamina _playerStamina;
	public override bool Use()
	{
		if (_playerStamina.Stamina == 100) 
			return false;
		_playerStamina.AddStamina(35);
		return true;
	}
}
