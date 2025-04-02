using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class ButtonOrderMechanism : MonoBehaviour, ISaveable
{
	[SerializeField]
	private string _interactableName;
	[SerializeField]
	private Interactable_Button[] _buttons;


	[SerializeField]
	private int[] _order;
	private int[] _objectStatus;
	private int _index;

	[SerializeField]
	private UnityEvent _trueEvents, _falseEvents, _atStartEvents, _doneEvents, _oneTimeEvent;

	private bool _startInitialized, _isDone, _checkTime;
	[SerializeField]
	private bool _isActive;

	private ButtonOrderMechanismData _data;


	[SerializeField]
	private TalkEvents[] _talkEvents;


	[Inject]
	private readonly SaveManager _saveManager;

	private void Awake()
	{
		_objectStatus = new int[_order.Length];
	}
	private void Start()
	{
		SetLoadFile();
	}
	public void SetLever(int id, bool isOn, bool atStart)
	{
		//if (atStart && !_startInitialized)
		//{
		//	_atStartEvents.Invoke();
		//	_startInitialized = true;
		//}
		//_objectStatus[id] = isOn;
		//CheckLevers(atStart);
	}

	public void EnableButton(int id)
	{
		_objectStatus[_index++] = id;
		if (_index == _order.Length)
			_checkTime = true;

		_index %= _order.Length;

		if (_checkTime)
		{
			Check();
			_checkTime = false;
		}
	}
	public void ResetMechanism()
	{
		for (int i = 0; i < _objectStatus.Length; i++)
		{
			_objectStatus[i] = 0;
			_buttons[i].SetStatusManually(false);
		}

		_index = 0;
	}

	public void Check()
	{
		if (_order.SequenceEqual(_objectStatus))
			ChangeStatus(true, false);
		else
		{
			ChangeStatus(false, false);
			PlayWrongSound();
			ResetMechanism();
		}
	}
	private void ChangeStatus(bool isActive, bool atStart)
	{
		if (_isActive == isActive) return;
		_isActive = isActive;

		if (isActive && !atStart)
		{
			if (!_isDone)
			{
				_isDone = true;
				_oneTimeEvent?.Invoke();
			}

			for (int i = 0; i < _buttons.Length; i++)
				_buttons[i].Lock();

			_trueEvents?.Invoke();
		}
		else if (isActive && atStart)
		{
			for (int i = 0; i < _buttons.Length; i++)
				_buttons[i].Lock();

			_doneEvents?.Invoke();
		}
		else
			_falseEvents.Invoke();
	}

	public GameData GetSaveFile()
	{
		_data = new ButtonOrderMechanismData()
		{
			MachineName = _interactableName,
			IsDone = _isDone,
			PressedArray = _objectStatus
		};
		return _data;
	}

	public void SetLoadFile()
	{
		_data = _saveManager.GetData<ButtonOrderMechanismData>(_interactableName);
		if (_data == null) return;

		_isDone = _data.IsDone;
		_objectStatus = _data.PressedArray;

		if (_isDone)
			ChangeStatus(true, true);
		else
			for (int i = 0; i < _objectStatus.Length; i++)
			{
				if (_objectStatus[i] != 0)
					_buttons[_objectStatus[i]].SetStatusManually(true);
			}
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
	}

	private void PlayWrongSound()
	{
		if (_talkEvents.Length > 0)
			_talkEvents[Random.Range(0, _talkEvents.Length)].SelectTalkList();
	}
}
