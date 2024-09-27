using System.Collections;
using System.Collections.Generic;
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
	private UnityEvent _onTriggerEvents;
	private bool _done;

	[Inject]
	private SaveManager _saveManager;

	private CEventData _data;

	private void Start()
	{
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
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
	}

	public void Setup()
	{
		gameObject.SetActive(!_done);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (_done && !_always)
			return;

		_done = true;
		_onTriggerEvents.Invoke();
		gameObject.SetActive(false);
	}
}

public class CEventData : GameData
{
	public bool Done;
}
