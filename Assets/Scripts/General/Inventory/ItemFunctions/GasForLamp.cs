using UnityEngine;

public class GasForLamp : UseFunction
{
	[SerializeField]
	private LanternHelpers _lanternHelpers;
	[SerializeField]
	private Lantern _lantern;
	public override bool Use()
	{
		if (_lanternHelpers.LeftFuel == _lanternHelpers.MaxFuel || !_lantern.IsPicked)
		{
			DialogueManager.Instance.PlayOne(_cantUse);
			return false;
		}

		_lanternHelpers.AddFuel(480f);
		return true;
	}
}
