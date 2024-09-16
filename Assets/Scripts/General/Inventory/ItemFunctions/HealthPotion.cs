using UnityEngine;

public class HealthPotion : UseFunction
{
	[SerializeField]
	private PlayerHealth _health;
	public override bool Use()
	{
		_health.AddHealth(35);
		return true;
	}
}
