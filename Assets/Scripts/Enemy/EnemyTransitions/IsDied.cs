using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class IsDied : AbstractCondition
{
	[SerializeField]
	private int _health;

	public override bool CheckCondition()
	{
		return _health == 0;
	}
	public void UpdateHealth(int damage)
	{
		_health += damage;
		if(_health < 0)
			_health = 0;
	}

    public override void ResetFrameFreeze() { }
}
