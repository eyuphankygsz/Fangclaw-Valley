
using UnityEngine;

public class TimeForExitFollow : AbstractCondition
{
	[SerializeField]
	private float _normalTime;
	private float _currentTime;

	public void ResetTime()
	{
		_currentTime = _normalTime;
	}
	public override bool CheckCondition()
	{
		if(_currentTime <= 0)
			return true;
		_currentTime -= Time.deltaTime;
		return false;
	}
}
