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
		if (_shakeRoutine != null)
			StopCoroutine(_shakeRoutine);

		_shakeRoutine = StartCoroutine(Shake(time));
	}
	public void SetShakeStrength(float strength)
		=> CameraShakerHandler.SetScaleAll(strength, true);

	private IEnumerator Shake(float time)
	{
		_shakeInstance = CameraShakerHandler.Shake(_shakeData);
		_shakeInstance.Data.SetShakeCanvases(true);

		yield return new WaitForSeconds(time);
		_shakeInstance.FadeOut();
		_shakeInstance = null;

	}
}
