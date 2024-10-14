using UnityEngine;

public class GasForLamp : UseFunction
{
	[SerializeField]
	private Lantern _lantern;
	public override bool Use()
	{
		if (_lantern.LeftFuel == _lantern.MaxFuel || !_lantern.IsPicked) 
			return false;

		_lantern.AddFuel(360);
		return true;
	}
}
