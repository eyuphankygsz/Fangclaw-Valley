using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Interactable_Button : Interactable
{
	public bool _oneTimePress;
	private bool _isPressed;

	[SerializeField]
	private UnityEvent _manuallyOn, _manuallyOff;

	[SerializeField]
	private Collider _collider;
	[SerializeField]
	private Transform _pressedTransform, _releaseTransform;
	[SerializeField]
	private AudioSource _src;
	[SerializeField]
	private AudioClip _turnOnSfx, _turnOffSfx;

	private ButtonData _buttonData;



	private bool _lock;

	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		//_src.Play();
		if ((_oneTimePress && _isPressed) || _lock)
			return;

		_collider.enabled = false;

		if (!_isPressed)
			Work();
		else if (_isPressed)
			Stop();

	}


	private void Work()
	{
		_isPressed = true;
		transform.DOMove(_pressedTransform.position, .2f).SetEase(Ease.Linear).OnComplete(AnimComplete);
		StartCoroutine(WorkRoutine());
	}
	private IEnumerator WorkRoutine()
	{
		PlayAudio(_turnOnSfx);

		while (!_collider.enabled)
			yield return null;

		_trueEvents?.Invoke();
	}


	private void Stop()
	{
		_isPressed = false;
		transform.DOMove(_releaseTransform.position, .2f).SetEase(Ease.Flash).OnComplete(AnimComplete);
		StartCoroutine(StopRoutine());
	}
	private IEnumerator StopRoutine()
	{
		PlayAudio(_turnOffSfx);
		while (!_collider.enabled)
			yield return null;

		_falseEvents?.Invoke();
	}


	private void AnimComplete()
	{
		_src.Stop();
		Debug.Log("STOPPPP AUDIO OMGG UR SO GAYYY");
		_collider.enabled = true;
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
			_isPressed = false;
			transform.DOMove(_releaseTransform.position, .2f).SetEase(Ease.Flash).OnComplete(AnimComplete);
			_manuallyOff?.Invoke();
		}
		else
		{
			_isPressed = true;
			transform.DOMove(_pressedTransform.position, .2f).SetEase(Ease.Flash).OnComplete(AnimComplete);
			_manuallyOn?.Invoke();
		}
		_collider.enabled = true;
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
		_buttonData = new ButtonData()
		{
			Name = InteractableName,
			IsPressed = _isPressed,
			IsLocked = _lock
		};
		return _buttonData;
	}

	public override void LoadData()
	{
		ButtonData data = _saveManager.GetData<ButtonData>(InteractableName);
		if (data == null) return;
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());

		_isPressed = data.IsPressed;
		_lock = data.IsLocked;
		if (_isPressed)
		{
			transform.DOMove(_pressedTransform.position, .2f).SetEase(Ease.Linear);
			_doneEvents?.Invoke();
		}
		//else
		//	Stop();


	}
}
