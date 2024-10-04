using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class MusicMechanism : MonoBehaviour, ISaveable
{
	[SerializeField]
	private string _order;
	[SerializeField]
	private StringBuilder _currentOrder = new StringBuilder();

	[SerializeField]
	private string _itemName;

	[SerializeField]
	private UnityEvent _onTrue;
	private bool _done;

	private MusicMechData _data;

	[Inject]
	private SaveManager _saveManager;

	private void Start()
	{
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
	}

	public void AddNote(char note)
	{
		if (_done) return;

		_currentOrder.Append(note);
		if (_currentOrder.Length > _order.Length)
			_currentOrder.Remove(0, _currentOrder.Length - _order.Length);

		CheckOrder();
	}

	public GameData GetSaveFile()
	{
		_data = new MusicMechData()
		{
			CurrentOrder = _currentOrder.ToString(),
			IsDone = _done,
		};
		return _data;
	}

	public void SetLoadFile()
	{
		_data = _saveManager.GetData<MusicMechData>(_itemName);
		if (_data == null) return;

		_done = _data.IsDone;
		if (_done)
		{
			_onTrue.Invoke();
			return;
		}
		_currentOrder.Append(_data.CurrentOrder);
	}

	private void CheckOrder()
	{
		if (_currentOrder.Equals(_order))
		{
			_onTrue.Invoke();
		}
	}
}
