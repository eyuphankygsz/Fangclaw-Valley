using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Interactable_PlaceHolder : Interactable
{
	[SerializeField]
	private InventoryItem _theItem;
	[SerializeField]
	private PoolItem _poolItem;
	[SerializeField]
	private Transform _holderTransform;
	[SerializeField]
	private bool _isFull;
	[SerializeField]
	private bool _startAsDisabled;
	private GameObject _holderObject;
	private StatusMechanism _mechanism;

	[Inject]
	private InventoryManager _inventoryManager;
	[Inject]
	private ObjectPool _objectPool;

	[SerializeField]
	private int _id;

	private bool _initialized;

	private PlaceHolderData _data;

	private void Start()
	{
		_mechanism = GetComponentInParent<StatusMechanism>();
		base.Start();
	}
	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		var item = _inventoryManager.GetItem(_theItem.ItemName.GetLocalizedString(), 1);
		if (!_isFull && item == null)
			return;

		HandleHolding(item);

	}
	private void OnEnable()
	{
		if(_initialized && _data.IsDisabled)
			gameObject.SetActive(false);
	}

	private void HandleHolding(InventoryItem item)
	{
		if (_isFull)
		{
			HandleHolderObject(placed: false, startAsDisabled: false);
			_inventoryManager.AddItemToInventory(_theItem, 1, null);
			_isFull = false;
		}
		else
		{
			HandleHolderObject(placed: true, startAsDisabled: false);
			_inventoryManager.RemoveItemQuantityFromInventory(_theItem, 1);
			_isFull = true;
		}

		_mechanism.SetLever(_id, _isFull, false);
	}
	private void HandleHolderObject(bool placed, bool startAsDisabled)
	{
		if (!placed && startAsDisabled && _holderObject != null)
			_holderObject.SetActive(false);

		_holderObject = null;

		if (placed && !startAsDisabled)
		{
			_holderObject = _objectPool.GetObject(_holderTransform.position, _poolItem);
			_holderObject.transform.rotation = transform.rotation;
			_holderObject.transform.SetParent(_holderTransform.parent);
		}
	}
	public override GameData GetGameData()
	{
		_data = new PlaceHolderData()
		{
			Name = InteractableName,
			IsItemOn = _isFull,
			IsDisabled = !gameObject.activeSelf
		};

		return _data;
	}

	public override void LoadData()
	{
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
		if (_initialized)
		{
			Debug.Log(_holderObject);
			return;
		}
		
		_initialized = true;
		var data = _saveManager.GetData<PlaceHolderData>(InteractableName);
		if (data == null)
		{
			gameObject.SetActive(!_startAsDisabled);
			return;
		}
		_data = data;
		gameObject.SetActive((_startAsDisabled && _data.IsDisabled) ? false : !_data.IsDisabled);
		HandleHolderObject(placed: data.IsItemOn, data.IsDisabled);
		_isFull = data.IsItemOn;
		_mechanism.SetLever(_id, _isFull, true);
	}

}