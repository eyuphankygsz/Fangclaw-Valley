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
}
