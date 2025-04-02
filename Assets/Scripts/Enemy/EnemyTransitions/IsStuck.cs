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

	private bool _firstTimeChecks;
	private int _attempts;

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
			if (Vector3.Distance(_lastPos, transform.position) == 0)
			{
				if (_stuckTimer.CheckCondition())
				{
					if(_attempts == 2)
					{
						_attempts = 0;
						return true;
					}
					_stuckTimer.ResetTime();
					_agent.ResetPath();
					Debug.Log("StuckTimer");
					//Debug.Log("Stucked!");
					//return true;
				}
			
				_stuck = true;
			}
			else
			{
				_stuck = false;
				_stuckTimer.ResetTime();
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
