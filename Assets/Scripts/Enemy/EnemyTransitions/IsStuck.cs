using UnityEngine;
using UnityEngine.AI;

public class IsStuck : AbstractCondition
{
	[SerializeField]
	private NavMeshAgent _agent;
	[SerializeField]
	private Vector3 _lastPos;

	[SerializeField]
	private float _stuckThreshold;
	[SerializeField]
	private TimeForExitStuck _stuckTimer;

	private bool _stuck;
	private bool _freeze;
	private bool _isTimerChecked;

	private bool _firstTimeChecks;

	public void SetLastPoint(Vector3 pos) { _lastPos = pos; }
	public override bool CheckCondition()
	{
		if (!_firstTimeChecks)
		{
			_stuckTimer.ResetTime();
			_firstTimeChecks = true;
		} 

		if (!_freeze)
		{
			if (Vector3.Distance(_lastPos, transform.position) <= .002f)
			{
				_stuckTimer.CheckCondition();
				_stuck = true;
				_isTimerChecked = true;
			}
			else
			{
				_stuck = false;
				_stuckTimer.ResetTime();
			}
			
			if (_isTimerChecked) 
			{
				if (_stuckTimer.CheckCondition())
					return true;

				_isTimerChecked = false;
			}

			_lastPos = transform.position;
		}
		_freeze = true;

		return false;
	}

	public bool GetStuck()
	{
		return _stuck;
	}
	public override void ResetFrameFreeze() { _freeze = false; }
}
