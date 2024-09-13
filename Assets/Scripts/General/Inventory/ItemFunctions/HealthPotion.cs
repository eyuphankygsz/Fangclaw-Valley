using UnityEngine;

public class HealthPotion : UseFunction
{
	[SerializeField]
	private PlayerHealth _health;
	public override void Use()
	{
		_health.AddHealth(35);
	}
}
