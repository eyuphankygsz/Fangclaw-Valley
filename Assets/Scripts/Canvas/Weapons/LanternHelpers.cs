using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanternHelpers : MonoBehaviour
{
	[SerializeField]
	private Image _uiFill;
	[SerializeField]
	private Lantern _lantern;

	private Coroutine _gasRoutine, _waveRoutine;
	private bool _waveOn;
	private bool _baseNormalLight;
	public float MaxFuel { get => _maxFuel; }
	public float LeftFuel
	{
		get => _leftFuel;
		set
		{
			_leftFuel = value;
			ApplyUI();
		}
	}
	public float LitMultiplier = 1;

	private float _leftFuel, _maxFuel = 480f;
	private bool _isOn;

	private void OnEnable()
	{
		if (_isOn)
		{
			if (_gasRoutine != null)
				StopCoroutine(_gasRoutine);
			StartUsingGas();
		}
	}

	#region Gas
	public bool IsGasEmpty()
	{
		if (_leftFuel <= 0)
		{
			StopUsingGas();
			_leftFuel = 0;
			ApplyUI();
			return true;
		}
		return false;
	}
	public void StartUsingGas()
	{
		if (_gasRoutine != null)
			StopCoroutine(_gasRoutine);
		_gasRoutine = StartCoroutine(UseGas());
	}
	public void StopUsingGas()
	{
		_isOn = false;
		if (_gasRoutine != null)
			StopCoroutine(_gasRoutine);
	}
	public IEnumerator UseGas()
	{
		_isOn = true;
		while (true)
		{
			_leftFuel -= Time.deltaTime * LitMultiplier;
			ApplyUI();

			if (_leftFuel <= 0)
			{
				_lantern.OnGasOut();
				break;
			}

			yield return null;
		}
	}
	public void AddFuel(float fuel)
	{
		_leftFuel += fuel;
		_leftFuel = _leftFuel > _maxFuel ? _maxFuel : _leftFuel;
		ApplyUI();

	}
	#endregion
	#region LightWave
	public void StartLightWave()
	{
		if (_waveRoutine != null)
			StopCoroutine(_waveRoutine);

		_baseNormalLight = _lantern.IsNormalLightOn();
		_waveRoutine = StartCoroutine(LightWave());
	}
	public void StopLightWave()
	{
		_waveing = false;
		StopCoroutine(_waveRoutine);
		_lantern.SetLightning(_lantern.IsShining);

		_lantern.Intensity(0, _lantern.GetInitialNormalIntensity());
		_lantern.Intensity(1, _lantern.GetInitialDirectIntensity());
		_lantern.Intensity(2, _lantern.GetInitialBehindIntensity());
		_lantern.Intensity(3, _lantern.GetInitialWeaponIntensity());
	}

	bool _waveing;
	public IEnumerator LightWave()
	{
		while (true)
		{
			bool onFire = _lantern.IsOnFire();
			if (!onFire)
			{
				_lantern.SetLightning(false);
				yield return null;
			}
			else
			{

				if (!_waveing)
				{
					_waveing = true;
					StartCoroutine(CalculateIntensity());
				}

			}
			yield return null;
		}
	}

	private IEnumerator CalculateIntensity()
	{
		bool enlight = _lantern.GetCurrentNormalIntensity() != _lantern.GetInitialNormalIntensity();

		float normalTime = Random.Range(0.1f, 0.7f), currentTime = 0;

		float currentNormal = _lantern.GetCurrentNormalIntensity();
		float currentDirect = _lantern.GetCurrentDirectIntensity();
		float currentBehind = _lantern.GetCurrentBehindIntensity();
		float currentWeapon = _lantern.GetCurrentWeaponIntensity();

		float targetNormal = enlight ? _lantern.GetInitialNormalIntensity() : _lantern.GetInitialNormalIntensity() * 0.1f;
		float targetDirect = enlight ? _lantern.GetInitialDirectIntensity() : _lantern.GetInitialDirectIntensity() * 0.1f;
		float targetBehind = enlight ? _lantern.GetInitialBehindIntensity() : _lantern.GetInitialBehindIntensity() * 0.1f;
		float targetWeapon = enlight ? _lantern.GetInitialWeaponIntensity() : _lantern.GetInitialWeaponIntensity() * 0.1f;
		float newIntensity = 0;


		while (currentTime != normalTime)
		{
			float progress = 1 - (currentTime / normalTime);
			if (progress > 1)
				progress = 1;

			for (int i = 0; i < 4; i++)
			{

				switch (i)
				{
					case 0:

						newIntensity = Mathf.Lerp(currentNormal, targetNormal, progress);
						break;

					case 1:

						newIntensity = Mathf.Lerp(currentDirect, targetDirect, progress);
						break;

					case 2:

						newIntensity = Mathf.Lerp(currentBehind, targetBehind, progress);
						break;
					case 3:

						newIntensity = Mathf.Lerp(currentWeapon, targetWeapon, progress);
						break;
				}
				_lantern.Intensity(i, newIntensity);

			}

			currentTime += Time.deltaTime;
			if (currentTime >= normalTime)
				currentTime = normalTime;
			yield return null;
		}

		if (_waveing)
		{
			_lantern.Intensity(0, targetNormal);
			_lantern.Intensity(1, targetDirect);
			_lantern.Intensity(2, targetBehind);
			_lantern.Intensity(3, targetWeapon);
			_waveing = false;
		}
	}
	#endregion

	private void ApplyUI()
	{
		_uiFill.fillAmount = Mathf.InverseLerp(0, _maxFuel, _leftFuel);
	}
}
