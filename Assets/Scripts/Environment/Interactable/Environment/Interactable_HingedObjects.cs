using UnityEngine;
using UnityEngine.Events;
using Zenject;

public class Interactable_HingedObjects : Interactable
{
	[Inject]
	InventoryManager _inventoryManager;

	[SerializeField]
	private LockKey _lockKey;

	[SerializeField]
	private UnityEvent _oneTimeEvents;
	[SerializeField]
	private UnityEvent _trueEvents;
	[SerializeField]
	private UnityEvent _falseEvents;

	[SerializeField]
	private bool _isOn;
	private bool _used;

	private bool _animating;
	private Animator _animator;

	private HingedData _data = new HingedData();

	private void Awake()
	{
		_animator = GetComponent<Animator>();
		base.Awake();

	}
	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);

		if (IsLocked() || _animating) return;

		SetDoorState(!_isOn);
	}
	public override void SetStatusManually(bool on)
	{
		SetDoorState(on);
	}
	public void AnimationOver()
	{
		_animating = false;
	}
	public void Unlock()
	{
		_lockKey.Locked = false;
		SetDoorState(true);
	}
	private bool IsLocked()
	{
		if (_lockKey.Locked)
		{
			var item = _inventoryManager.GetItem(_lockKey.KeyName, 1);
			if (item != null)
			{
				_lockKey.Locked = false;
				_inventoryManager.RemoveItemFromInventory(item);
				return false;
			}
			Debug.Log("LOCKED");
			return true;
		}

		return false;
	}
	public override GameData GetGameData()
	{
		_data = new HingedData
		{
			Name = InteractableName,
			IsLocked = _lockKey.Locked,
			IsOn = _isOn,
			Used = _used
		};
		return _data;
	}

	public override void LoadData()
	{
		HingedData data = _saveManager.GetData<HingedData>(InteractableName);
		if (data == null) return;
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());

		if (data.Used)
			OneTimeEvent();

		_isOn = data.IsOn;
		_lockKey.Locked = data.IsLocked;
		// Assume you have a method to set the door state directly based on _isOn
		SetDoorState(_isOn);
	}

	private void SetDoorState(bool isOn)
	{
		if (isOn)
			OneTimeEvent();

		_isOn = isOn;
		_animator.SetBool("On", _isOn);
		_animating = _isOn;
	}

	private void OneTimeEvent()
	{
		if (!_used)
		{
			_used = true;
			_oneTimeEvents.Invoke();
		}

	}
}
