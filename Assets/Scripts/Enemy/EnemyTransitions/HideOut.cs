using UnityEngine;

public class HideOut : AbstractCondition
{
	[SerializeField]
	private IsPlayerHiding _pHide;

	private bool _canHideAttack;

	public void SetHideAttack()
	{
		_canHideAttack = true;
	}
	public override bool CheckCondition()
	{
		if (!_pHide.CheckCondition())
			_canHideAttack = false;

		return _canHideAttack;
	}

	public override void ResetFrameFreeze()
	{
	}
}
