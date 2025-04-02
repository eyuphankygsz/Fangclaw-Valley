using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class CustomEvents : MonoBehaviour, ISaveable
{
	[SerializeField]
	private int _eventID;
	[SerializeField]
	private bool _always;

	[SerializeField]
	private bool _notOnChase;

	[SerializeField]
	private UnityEvent _onTriggerEvents, _doneEvents;
	private bool _done;

	[Inject]
	private SaveManager _saveManager;
	[Inject]
	private GameManager _manager;

	private CEventData _data;

	private void Start()
	{
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
		SetLoadFile();
	}
	public GameData GetSaveFile()
	{
		_data = new CEventData()
		{
			Name = _eventID.ToString(),
			Done = _done
		};
		return _data;
	}
	public void SetLoadFile()
	{
		var data = _saveManager.GetData<CEventData>(_eventID.ToString());
		_saveManager.AddSaveableObject(gameObject, _data);

		if (data == null)
			return;

		_done = data.Done;
		if (_done)
			_doneEvents?.Invoke();
	}

	public void Setup()
	{
		gameObject.SetActive(!_done);
	}
	private void OnTriggerEnter(Collider other)
	{
		if(_notOnChase)
			if(_manager != null && _manager.IsOnChase > 0)
			return;

		if (other.tag != "Player")
			return;

		if (_done && !_always)
			return;

		_done = true;
		_onTriggerEvents.Invoke();
		gameObject.SetActive(false);
	}
	public void SetDoneManually(bool done)
	{
		_done = true;
		gameObject.SetActive(false);
	}
	public UnityEvent GetEvents() => _onTriggerEvents;
}

public class CEventData : GameData
{
	public bool Done;
}
