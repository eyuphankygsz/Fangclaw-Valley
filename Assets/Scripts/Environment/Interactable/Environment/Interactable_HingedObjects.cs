using UnityEngine;
using Zenject;

public class Interactable_HingedObjects : Interactable
{
	[Inject]
	InventoryManager _inventoryManager;
	
	[SerializeField]
	private LockKey _lockKey;

	[SerializeField]
	private bool _isOn;
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

	public void AnimationOver()
	{
		_animating = false;
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
			IsOn = _isOn
		};
		return _data;
	}

	public override void LoadData()
	{
		HingedData data = _saveManager.GetData<HingedData>(InteractableName);
		if (data == null) return;
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());

		_isOn = data.IsOn;
		_lockKey.Locked = data.IsLocked;
		// Assume you have a method to set the door state directly based on _isOn
		SetDoorState(_isOn);
	}

	private void SetDoorState(bool isOn)
	{
		_isOn = isOn;
		_animator.SetBool("On", _isOn);
		_animating = _isOn;
	}
}
