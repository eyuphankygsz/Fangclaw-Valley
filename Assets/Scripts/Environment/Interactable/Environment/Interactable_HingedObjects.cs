using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Interactable_HingedObjects : Interactable
{
	[SerializeField]
	private LockKey _lockKey;

	[SerializeField]
	private bool _isOn;
	private bool _animating;
	private Animator _animator;

	private void Awake()
	{
		base.Awake();
		_animator = GetComponent<Animator>();

	}
	public override void OnInteract(Enum_Weapons weapon)
	{
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
			var item = InventoryManager.Instance.GetItem(_lockKey.KeyName, 1);
			if (item != null)
			{
				_lockKey.Locked = false;
				InventoryManager.Instance.RemoveItemFromInventory(item);
				return false;
			}
			return true;
		}

		return false;
	}
	public override InteractableData SaveData()
	{
		return new HingedData
		{
			InteractableName = InteractableName,
			Position = transform.position,
			IsActive = gameObject.activeSelf,
			IsOn = _isOn,
			IsLocked = _lockKey.Locked
		};
	}

	public override void LoadData()
	{
		HingedData data = (HingedData)SaveManager.Instance.GetData(InteractableName, SaveType.Hinged);
		if (data == null) return;

		transform.position = data.Position;
		gameObject.SetActive(data.IsActive);
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
