using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
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

	[SerializeField]
	private AudioClip[] _openClips, _closeClips, _lockedClips;
	[SerializeField]
	private AudioSource _source;


	private NavMeshObstacle _navObstacle;
	private void Awake()
	{
		_animator = GetComponent<Animator>();
		TryGetComponent<NavMeshObstacle>(out _navObstacle);

		base.Awake();

	}

	private void Start()
	{
		base.Start();
	}
	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);

		if (CheckLock() || _animating) return;

		SetDoorState(!_isOn, false, atStart: false);
	}
	public override void SetStatusManually(bool on) => SetDoorState(on, false, atStart: false);
	public void SetStatusManuallySilent(bool on) => SetDoorState(on, true, atStart: false);
	public bool GetStatus() => _isOn;
	public void AnimationOver()
	{
		_animating = false;
	}
	public void Unlock(bool silent)
	{
		_lockKey.Locked = false;
		SetDoorState(true, silent, atStart: false);
	}

	public bool IsLocked() => _lockKey.Locked;
	private bool CheckLock()
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
			PlayClip(_lockedClips);
			return true;
		}

		return false;
	}

	private void PlayClip(AudioClip[] clips)
	{
		if (clips.Length > 0)
		{
			_source.clip = clips[Random.Range(0, clips.Length)];
			_source.Play();
		}
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
			DoneEvent();

		_isOn = data.IsOn;
		_lockKey.Locked = data.IsLocked;
		// Assume you have a method to set the door state directly based on _isOn
		SetDoorState(_isOn, true, true);
	}

	private void SetDoorState(bool isOn, bool silent, bool atStart)
	{
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());

		if (isOn && !atStart)
			OneTimeEvent();
		else if (isOn && atStart)
			DoneEvent();

		_isOn = isOn;
		if (!silent)
			PlayClip(_isOn ? _openClips : _closeClips);
		_animator.SetBool("On", _isOn);
		_animating = _isOn;

		//if (_navObstacle != null)
		//	_navObstacle.carving = !_isOn;
	}


}
