using UnityEngine;

public class StaminaPotion : UseFunction
{
	[SerializeField]
	private PlayerStamina _stamina;
	public override void Use()
	{
		_stamina.AddStamina(35);
	}
}
