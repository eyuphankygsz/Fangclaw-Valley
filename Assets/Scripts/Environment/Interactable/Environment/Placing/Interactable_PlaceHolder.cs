using UnityEngine;
using Zenject;

public class Interactable_PlaceHolder : Interactable
{
	[SerializeField]
	private InventoryItem _theItem;
	[SerializeField]
	private InventoryItem _poolItem;
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
	private int _statusID;
	private PlaceHolderData _data;

	private bool _isLocked;
	public bool IsLocked
	{
		get { return _isLocked; }

		set
		{
			_isLocked = value;
			GetComponent<Collider>().enabled = !_isLocked;
		}
	}

#pragma warning disable CS0108
	private void Start()
#pragma warning restore CS0108
	{
		_mechanism = GetComponentInParent<StatusMechanism>();
		base.Start();

		if (gameObject.activeSelf && _statusID == 0)
			_statusID = 2;
	}
	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		var item = _inventoryManager.GetItem(_theItem.ItemName.GetLocalizedString(), 1);
		if (!_isFull && item == null)
			return;

		HandleHolding(item);

	}

	public void EnablePlaceholder(bool enable)
	{
		_statusID = enable ? 2 : 1;
		gameObject.SetActive(enable);
	}
	private void OnEnable()
	{
		if (_initialized && _statusID == 1)
			gameObject.SetActive(false);
	}

	private void HandleHolding(InventoryItem item)
	{
		if (_isFull)
		{
			HandleHolderObject(placed: false);
			_inventoryManager.AddItemToInventory(_theItem, 1, null);
			_isFull = false;
		}
		else
		{
			HandleHolderObject(placed: true);
			_inventoryManager.RemoveItemQuantityFromInventory(_theItem, 1);
			_isFull = true;
		}

		_mechanism.SetLever(_id, _isFull, false);
	}
	private void HandleHolderObject(bool placed)
	{
		if (_statusID != 0)
			_statusID = placed ? 1 : 2;
		if (!placed && _holderObject != null)
			_holderObject.SetActive(false);

		_holderObject = null;

		if (placed)
		{
			Debug.Log("PLACED");
			_trueEvents?.Invoke();
			_holderObject = _objectPool.GetObject(_holderTransform.position, _poolItem);
			_holderObject.transform.rotation = transform.rotation;
			_holderObject.transform.SetParent(_holderTransform.parent);
		}
		else
		{
			Debug.Log("REMOVED");
			_falseEvents?.Invoke();
		}
	}
	public override GameData GetGameData()
	{
		_data = new PlaceHolderData()
		{
			Name = InteractableName,
			IsItemOn = _isFull,
			StatusID = _statusID
		};

		return _data;
	}

	public override void LoadData()
	{
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
		if (_initialized)
			return;

		_initialized = true;
		var data = _saveManager.GetData<PlaceHolderData>(InteractableName);
		if (data == null)
		{
			gameObject.SetActive(!_startAsDisabled);
			return;
		}
		_data = data;
		_statusID = data.StatusID;
		gameObject.SetActive((_startAsDisabled && _statusID != 2) ? false : _statusID == 2 ? true : false);
		HandleHolderObject(placed: data.IsItemOn);
		_isFull = data.IsItemOn;
		_mechanism.SetLever(_id, _isFull, _statusID == 2 ? false : true);
	}

}