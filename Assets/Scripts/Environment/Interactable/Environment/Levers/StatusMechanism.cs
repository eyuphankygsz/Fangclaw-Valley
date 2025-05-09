using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class StatusMechanism : MonoBehaviour, ISaveable
{
	[SerializeField]
	private string _interactableName;

	[SerializeField]
	private bool[] _order;
	[SerializeField]
	private UnityEvent _trueEvents, _falseEvents, _atStartEvents, _doneEvents, _oneTimeEvent;

	private bool _startInitialized, _oneTimeBool;
	[SerializeField]
	private bool[] _objectStatus;
	private bool _isActive;


	private StatusMechanismData _data;

	[Inject]
	private readonly SaveManager _saveManager;

	private void Awake()
	{
		_objectStatus = new bool[_order.Length];
	}
	private void Start()
	{
		SetLoadFile();
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
	public void SetOrderTrue(int id)
	{
		_objectStatus[id] = true;
		CheckLevers(false);
	}
	public void SetOrderFalse(int id)
	{
		_objectStatus[id] = false;
		CheckLevers(false);
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
		{
			if (!_oneTimeBool)
			{
				_oneTimeBool = true;
				_oneTimeEvent?.Invoke();
			}

			_trueEvents?.Invoke();
		}
		else if (isActive && atStart)
			_doneEvents?.Invoke();
		else
			_falseEvents.Invoke();
	}

	public GameData GetSaveFile()
	{
		_data = new StatusMechanismData()
		{
			MachineName = _interactableName,
			OneTime = _oneTimeBool
		};
		return _data;
	}

	public void SetLoadFile()
	{
		_data = _saveManager.GetData<StatusMechanismData>(_interactableName);
		if (_data == null) return;

		_oneTimeBool = _data.OneTime;
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
	}
}
