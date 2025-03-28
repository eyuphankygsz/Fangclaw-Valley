
using UnityEngine;

public class CanCheckHitBox : AbstractCondition
{
	[SerializeField]
	private bool _canHit;
	public override bool CheckCondition()
	{
		return _canHit;
	}

	public void SetCanHit(bool canHit) => _canHit = canHit;

	public override void ResetFrameFreeze()
	{

	}
}
