using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class PlayerUI : MonoBehaviour
{
	[SerializeField]
	private Image _staminaBar, _healthBar;
	[SerializeField]
	private float _maxHealth, _maxStamina;

	[SerializeField]
	private ShakeData _shakeData = null;
	private Coroutine _shakeRoutine;
	private ShakerInstance _shakeInstance;




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
		Debug.Log("StartShake");
		if (_shakeRoutine != null)
		{
			Debug.Log("SHAKEROUTINE STOP");

			_shakeInstance.FadeOut(0);
			StopCoroutine(_shakeRoutine);
		}

		_shakeRoutine = StartCoroutine(Shake(time));
	}
	public void StopShake()
	{
		Debug.Log("SHAKEROUTINE STOP2");
		_shakeInstance.FadeOut(1);
	}
	public void SetShakeStrength(float strength)
		=> CameraShakerHandler.SetScaleAll(strength, true);

	private IEnumerator Shake(float time)
	{
		_shakeInstance = CameraShakerHandler.Shake(_shakeData);
		_shakeInstance.Data.SetShakeCanvases(true);

		yield return new WaitForSeconds(time);
		if (time != 0)
			_shakeInstance.FadeOut(1);
	}
}
