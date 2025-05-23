using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[SerializeField]
	private Image _staminaBar, _healthBar;
	[SerializeField]
	private float _maxHealth, _maxStamina;

	[SerializeField]
	private ShakeData _shakeData = null;
	private Coroutine _shakeRoutine;
	private List<ShakerInstance> _shakeInstances;

	[SerializeField]
	private PlayerCamera _playerCamera;

	[SerializeField]
	private IsDied _playerIsDied;

	public void SetMaxHealth(float maxHealth)
	{
		_maxHealth = maxHealth;
	}
	public void SetMaxStamina(float maxStamina)
	{
		_maxStamina = maxStamina;
	}
	public void ChangeHealthBar(float value)
	{
		_healthBar.fillAmount = CalculateFillAmount(value, _maxHealth);
		if(value <= 0)
		_playerIsDied.UpdateHealth(-100);
	}
	public void ChangeStaminaBar(float value)
	{
		_staminaBar.fillAmount = CalculateFillAmount(value, _maxStamina);
	}

	private float CalculateFillAmount(float value, float maxValue)
	{
		return Mathf.InverseLerp(0, maxValue, value);
	}
	public void StartShake(float time)
	{
		//Debug.Log("StartShake");
		if (_shakeRoutine != null)
		{
			//Debug.Log("SHAKEROUTINE STOP");
			foreach (ShakerInstance shaker in _shakeInstances)
			{
				shaker.FadeOut(0);
			}
			StopCoroutine(_shakeRoutine);
		}

		_shakeRoutine = StartCoroutine(Shake(time));
	}
	public void StopShake()
	{
		//Debug.Log("SHAKEROUTINE STOP2");
		foreach (ShakerInstance shaker in _shakeInstances)
		{
			shaker.FadeOut(1);
		}
		_playerCamera.LockRandom = false;
	}
	public void SetShakeStrength(float strength)
		=> CameraShakerHandler.SetScaleAll(strength, true);

	private IEnumerator Shake(float time)
	{
		_playerCamera.LockRandom = true;

		if(_shakeInstances != null)
			foreach (ShakerInstance shaker in _shakeInstances)
			{
				shaker.Stop();
			}
	    _shakeInstances = CameraShakerHandler.ShakeAll(_shakeData);
		_shakeInstances[0].Data.SetShakeCanvases(true);

		if (time != 0)
		{
			yield return new WaitForSeconds(time);
			foreach (ShakerInstance shaker in _shakeInstances)
			{
				shaker.FadeOut(1);
			}
			_playerCamera.LockRandom = false;
		}
	}
}
