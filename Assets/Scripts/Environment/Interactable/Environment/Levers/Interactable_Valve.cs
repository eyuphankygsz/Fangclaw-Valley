using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Interactable_Valve : Interactable
{
	[SerializeField]
	private bool _oneTimeDone;
	private bool _isDone;

	[SerializeField]
	private UnityEvent _manuallyOn, _manuallyOff;

	[SerializeField]
	private AudioSource _src;
	[SerializeField]
	private AudioClip _turnOnSfx, _turnOffSfx;

	[SerializeField]
	private float _rotateAngle, _rotateSpeed;
	private float _currentRot;

	[SerializeField]
	private bool _rotating;

	[SerializeField]
	private bool _lock;
	private Coroutine _routine;

	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		//_src.Play();
		if (_lock || _isDone || _rotating)
			return;

		_rotating = true;
		Work();
	}
	public override void OnStopInteract(Enum_Weapons weapon)
	{
		if (_lock || !_rotating || _isDone)
			return;

		_rotating = false;
		Stop();
	}

	private void Work()
	{
		if (_oneTimeDone && _isDone)
			return;
		_src.clip = _turnOnSfx;
		_src.Play();
		_routine = StartCoroutine(WorkRoutine());
	}
	private IEnumerator WorkRoutine()
	{
		while (_currentRot < _rotateAngle)
		{
			_currentRot += _rotateSpeed * Time.deltaTime;
			SetRotation(); 
			yield return null;
		}
		_currentRot = _rotateAngle;
		_trueEvents?.Invoke();
		SetRotation();

	}
	private void SetRotation()
	{
		transform.localRotation = Quaternion.Euler(0, 0, _currentRot);
	}

	private void Stop()
	{
		StopCoroutine(_routine);
		_src.clip = _turnOnSfx;
		_src.Play();
		_routine = StartCoroutine(BackToNormal());
	}
	private IEnumerator BackToNormal()
	{
		_falseEvents?.Invoke();
		while (_currentRot > 0)
		{
			_currentRot -= _rotateSpeed * Time.deltaTime;
			SetRotation();
			yield return null;
		}
		_currentRot = 0;
		SetRotation();
	}
	private void PlayAudio(AudioClip clip)
	{
		_src.clip = clip;
		_src.Play();
	}

	public override void SetStatusManually(bool on)
	{
		base.SetStatusManually(on);

		if (!on)
		{
			_isDone = false;
			_currentRot = 0;
			SetRotation();
			_manuallyOff?.Invoke();
		}
		else
		{
			_isDone = true;
			_currentRot = _rotateAngle;
			SetRotation();
			_manuallyOn?.Invoke();
		}
	}

	public void Lock()
	{
		_lock = true;
	}
	public void UnLock()
	{
		_lock = false;
	}

	public override GameData GetGameData()
	{
		return new ValveData()
		{
			Name = InteractableName,
			IsDone = _isDone,
			IsLocked = _lock
		};

	}

	public override void LoadData()
	{
		ValveData data = _saveManager.GetData<ValveData>(InteractableName);
		if (data == null) return;
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());

		_isDone = data.IsDone;
		_lock = data.IsLocked;
		if (_isDone)
		{
			_currentRot = _rotateAngle;
			SetRotation();

			_doneEvents?.Invoke();
		}
	}
}
