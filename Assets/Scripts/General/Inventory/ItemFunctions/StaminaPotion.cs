using UnityEngine;

public class StaminaPotion : UseFunction
{
	[SerializeField]
	private PlayerStamina _stamina;
	public override bool Use()
	{
		_stamina.AddStamina(35);
		return true;
	}
}
