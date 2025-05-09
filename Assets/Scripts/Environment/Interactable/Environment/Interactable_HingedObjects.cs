using System.Collections;
using UnityEngine;
using UnityEngine.AI;
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


	private WaitForSeconds _wfs = new WaitForSeconds(0.1f);
	private AnimatorStateInfo _animatorStateInfo;

	private NavMeshObstacle _navObstacle;

	[SerializeField]
	private TalkEvents[] _talkEvents;


	private bool _lockSaid;



#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	private void Awake()
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
	{
		_animator = GetComponent<Animator>();
		TryGetComponent<NavMeshObstacle>(out _navObstacle);

		base.Awake();

	}

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	private void Start()
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
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
	public void Lock(bool silent)
	{
		_lockKey.Locked = true;
		SetDoorState(false, silent, atStart: false);
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
			TryPlayLocked();
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
	private void TryPlayLocked()
	{
		if (!_lockSaid)
		{
			_talkEvents[Random.Range(0, _talkEvents.Length)].SelectTalkList();
			_lockSaid = true;
			StartCoroutine(ResetLockSaid());
		}
	}
	private IEnumerator ResetLockSaid()
	{
		yield return new WaitForSeconds(Random.Range(3,4));
		_lockSaid = false;
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

		_used = data.Used;
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
		if (_isOn)
			_trueEvents?.Invoke();
		else
			_falseEvents?.Invoke();

		if (!silent)
			PlayClip(_isOn ? _openClips : _closeClips);
		_animator.SetBool("On", _isOn);
		_animating = _isOn;

		//if (_navObstacle != null)
		//	_navObstacle.carving = !_isOn;
	}
	private Coroutine _animationCheckRoutine;
	public void StartAnimationCheck(string name)
	{
		if (_animationCheckRoutine != null)
			StopCoroutine(_animationCheckRoutine);
		_animationCheckRoutine = StartCoroutine(CheckAnimation(name));
	}
	private IEnumerator CheckAnimation(string name)
	{
		_animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
		if (!_animatorStateInfo.IsName(name))
			yield return _wfs;

		while (true)
		{
			_animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
			if (!_animatorStateInfo.IsName(name))
			{
				AfterDoneEvents();
				break;
			}
			else if (_animatorStateInfo.normalizedTime >= _animatorStateInfo.length)
			{
				AfterDoneEvents();
				break;
			}
			yield return null;
		}
	}

	private void AfterDoneEvents()
	{
		if (_isOn)
			_trueDoneEvents?.Invoke();
		else
			_falseDoneEvents?.Invoke();
	}


}
