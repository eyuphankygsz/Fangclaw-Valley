using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatusMechanism : MonoBehaviour
{
	[SerializeField]
	private bool[] _order;
	[SerializeField]
	private UnityEvent _trueEvents, _falseEvents, _atStartEvents, _doneEvents;

	private bool _startInitialized;
	[SerializeField]
	private bool[] _objectStatus;
	private bool _isActive;

	private void Awake()
	{
		_objectStatus = new bool[_order.Length];
	}
	public void SetLever(int id, bool isOn, bool atStart)
	{
		if (atStart && !_startInitialized)
		{
			_atStartEvents.Invoke();
			_startInitialized = true;
		}
		_objectStatus[id] = isOn;
		CheckLevers(atStart);
	}

	private void CheckLevers(bool atStart)
	{
		for (int i = 0; i < _order.Length; i++)
			if (_objectStatus[i] != _order[i])
			{
				ChangeStatus(false, atStart);
				return;
			}
		ChangeStatus(true, atStart);
	}

	private void ChangeStatus(bool isActive, bool atStart)
	{
		if (_isActive == isActive) return;
		_isActive = isActive;

		if (isActive && !atStart)
			_trueEvents?.Invoke();
		else if (isActive && atStart)
			_doneEvents?.Invoke();
		else
			_falseEvents.Invoke();
	}
}
