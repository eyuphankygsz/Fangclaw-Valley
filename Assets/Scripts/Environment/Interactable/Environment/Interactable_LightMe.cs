using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Interactable_LightMe : Interactable
{
	private LightMeData _data;

	[SerializeField]
	private float _increaseSpeed, _decreaseSpeed, _maxTime, _currentTime;
	private bool _full, _onWait, _oneTimeDone;

	private Coroutine _routine, _ticksRoutine;

	[SerializeField]
	private UnityEvent _ticksEvents, _ticksDoneEvents, _ticksResetEvents;

	[SerializeField]
	private Image _fill;

	[SerializeField]
	private float _decreaseWaitTime = 3f, _restoreWaitTime = 10;
	private WaitForSeconds _decreaseWFS, _restoreWFS, _oneWFS = new WaitForSeconds(1);
	private void Awake()
	{
		base.Awake();
		_decreaseWFS = new WaitForSeconds(_decreaseWaitTime);
		_restoreWFS = new WaitForSeconds(_restoreWaitTime);
	}
	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		if (_onWait) return;
		if (_routine != null)
			StopCoroutine(_routine);

		ChangeBattery(_increaseSpeed);
	}
	public override void OnStopInteract(Enum_Weapons weapon)
	{
		base.OnStopInteract(weapon);
		if (_full || _onWait) return;

		if (_routine != null)
			StopCoroutine(_routine);

		_routine = StartCoroutine(DecreaseRoutine());
	}
	private void ChangeFillAmount()
	{
		if (_fill == null)
			return;

		_fill.fillAmount = Mathf.InverseLerp(0, _maxTime, _currentTime);
	}
	private IEnumerator DecreaseRoutine()
	{
		bool wasFul = _full;
		if (_ticksRoutine != null)
			StopCoroutine(_ticksRoutine);


		if (_full)
		{
			_ticksRoutine = StartCoroutine(TicksRoutine());
			yield return _decreaseWFS;
			wasFul = true;
		}

		while (_currentTime > 0)
		{
			yield return null;
			ChangeBattery(_decreaseSpeed);
		}
		if (wasFul)
		{
			yield return _restoreWFS;
			_onWait = false;
		}
	}
	private IEnumerator TicksRoutine()
	{
		while (_currentTime >= 1)
		{
			yield return _oneWFS;
			Debug.Log(_currentTime);
			_ticksEvents?.Invoke();
		}
		_ticksDoneEvents?.Invoke();
	}
	public void ChangeBattery(float speed)
	{
		_currentTime += speed * Time.deltaTime;


		if (_currentTime >= _maxTime)
		{
			_currentTime = _maxTime;
			if (!_full)
			{
				_onWait = true;
				_full = true;

				if (_routine != null)
					StopCoroutine(_routine);
				_routine = StartCoroutine(DecreaseRoutine());
				
				if(!_oneTimeDone)
				{
					_oneTimeEvents?.Invoke();
					_oneTimeDone = true;
				}
				_trueEvents?.Invoke();
				
			}
		}
		else if (_currentTime <= 0)
		{
			_currentTime = 0;
			if (_full)
			{
				_full = false;
				_falseEvents?.Invoke();

			}
		}
		ChangeFillAmount();
	}

	public override GameData GetGameData()
	{
		_data = new LightMeData()
		{
			Name = InteractableName,
			IsOneTimeDone = _oneTimeDone
		};
		return _data;
	}

	public override void LoadData()
	{
		LightMeData data = _saveManager.GetData<LightMeData>(InteractableName);
		if (data == null) return;

		_oneTimeDone = data.IsOneTimeDone;
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
	}
}