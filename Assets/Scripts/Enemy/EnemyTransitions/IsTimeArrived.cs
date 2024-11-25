
using System;
using UnityEngine;

public class IsTimeArrived : AbstractCondition
{
	[SerializeField]
	private CustomTimeSpan _startSpan, _endSpan;
	[SerializeField]
	private GameTime _gameTime;

	private TimeSpan _time;
	private bool _freeze;
	private bool _freezeTime;

	private float _freezeTimeFloat;



	public void SetFreeze(bool freeze) => _freeze = freeze;
	public void SetTime(float time) => _freezeTimeFloat = time;

	public override bool CheckCondition()
	{
		CheckTime();

		_time = _gameTime.GetTime();
		return (_time.Hours > _startSpan.GetTime().Hours && _time.Minutes > _startSpan.GetTime().Minutes)
			&& (_time.Hours < _endSpan.GetTime().Hours && _time.Minutes < _endSpan.GetTime().Minutes);
	}
	public void CheckTime()
	{
		if (_freeze)
		{
			if (!_freezeTime)
			{
				_freezeTimeFloat -= Time.deltaTime;
				if (_freezeTimeFloat <= 0)
					_freeze = false;
			}
			_freezeTime = true;

		}
	}
	public override void ResetFrameFreeze()
	{
		_freezeTime = false;
	}

}
[System.Serializable]
public class CustomTimeSpan
{
	public int Hour;
	public int Minute;
	public TimeSpan GetTime()
	{
		return new TimeSpan(Hour, Minute, 0);
	}
}