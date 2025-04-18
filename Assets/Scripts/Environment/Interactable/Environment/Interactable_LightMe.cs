using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Interactable_LightMe : Interactable
{
	[SerializeField]
	private Enum_Weapons _weapon;

	private WeaponPickData _data;

	[SerializeField]
	private float _increaseSpeed, _decreaseSpeed, _maxTime, _currentTime;
	private bool _full, _onWait;

	private Coroutine _routine;
	[SerializeField]
	private Image _fill;

	[SerializeField]
	private float _decreaseWaitTime = 3f, _restoreWaitTime = 10;
	private WaitForSeconds _decreaseWFS, _restoreWFS;
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
		if (_full)
		{
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
		_data = new WeaponPickData()
		{
			Name = InteractableName,
			IsTaken = !gameObject.activeSelf
		};
		return _data;
	}

	public override void LoadData()
	{
		WeaponPickData data = _saveManager.GetData<WeaponPickData>(InteractableName);
		if (data == null) return;

		gameObject.SetActive(!data.IsTaken);
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
	}
}