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
	private bool _full;

	private Coroutine _routine;
	[SerializeField]
	private Image _fill;
	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		ChangeBattery(_increaseSpeed);

		if (_routine != null)
			StopCoroutine(_routine);
	}
	public override void OnStopInteract(Enum_Weapons weapon)
	{
		base.OnStopInteract(weapon);

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
		while (_currentTime > 0)
		{
			yield return null;
			ChangeBattery(_decreaseSpeed);
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
				_trueEvents?.Invoke();
				_full = true;
			}
		}
		else if (_currentTime <= 0)
		{
			_currentTime = 0;
			if (_full)
			{
				_falseEvents?.Invoke();
				_full = false;
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