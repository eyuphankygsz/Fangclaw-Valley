using UnityEngine;

public class GasForLamp : UseFunction
{
	[SerializeField]
	private LanternHelpers _lanternHelpers;
	[SerializeField]
	private Lantern _lantern;
	public override bool Use()
	{
		Debug.Log(_lanternHelpers.LeftFuel + "  " + _lanternHelpers.MaxFuel);
		if (_lanternHelpers.LeftFuel == _lanternHelpers.MaxFuel || !_lantern.IsPicked) 
			return false;

		_lanternHelpers.AddFuel(240f);
		return true;
	}
}
