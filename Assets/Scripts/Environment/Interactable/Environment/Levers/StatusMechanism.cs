using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StatusMechanism : MonoBehaviour
{
	[SerializeField]
	private bool[] _order;
	[SerializeField]
	private UnityEvent _trueEvents, _falseEvents;

	private bool[] _objectStatus;
	private bool _isActive;

	private void Awake()
	{
		_objectStatus = new bool[_order.Length];
	}
	public void SetLever(int id, bool isOn)
	{
		_objectStatus[id] = isOn;
		CheckLevers();
	}

	private void CheckLevers()
	{
		for (int i = 0; i < _order.Length; i++)
			if (_objectStatus[i] != _order[i])
			{
				ChangeStatus(false);
				return;
			}
		ChangeStatus(true);
	}

	private void ChangeStatus(bool isActive)
	{
		if (_isActive == isActive) return;
		_isActive = isActive;

		if (isActive)
			_trueEvents.Invoke();
		else
			_falseEvents.Invoke();
	}
}
