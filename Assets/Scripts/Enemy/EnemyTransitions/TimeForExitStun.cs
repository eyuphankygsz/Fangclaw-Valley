using UnityEngine;

public class TimeForExitStun : AbstractCondition
{
	[SerializeField]
	private float _normalTime;
	private float _currentTime;

	private bool _freeze;
	private bool _firstTime;
	public void ResetTime()
	{
		_currentTime = _normalTime;
	}
	public override bool CheckCondition()
	{
		if (!_firstTime)
			_currentTime = _normalTime;
		_firstTime = true;

		if (_currentTime <= 0)
			return true;

		if (!_freeze)
			_currentTime -= Time.deltaTime;
		_freeze = true;

		return false;
	}

	public override void ResetFrameFreeze()
	{
		_freeze = false;
	}
}
