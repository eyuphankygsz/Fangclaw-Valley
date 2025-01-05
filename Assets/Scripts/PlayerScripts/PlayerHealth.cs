using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerHealth : MonoBehaviour
{
	[SerializeField]
	private float _health, _maxHealth;

	[Inject]
	private PlayerUI _playerUI;

	[SerializeField]
	private CameraAnimator _camAnim;

	public float Health
	{
		get => _health;
		set
		{
			_health = value;
		}
	}

	private void Start()
	{
		_playerUI.SetMaxHealth(_maxHealth);
	}
	public void AddHealth(float addedHealth)
	{
		if (addedHealth < 0)
			LoseEffects();
		_health = Mathf.Clamp(_health + addedHealth, 0, _maxHealth);
		_playerUI.ChangeHealthBar(_health);
	}
	private void LoseEffects()
	{
		_playerUI.SetShakeStrength(3);
		_playerUI.StartShake(1);

		_camAnim.PlayAnimation("Hit");
	}
	public void SetHealth(float health)
	{
		_health = health;
		ChangeUI();
	}
	private void ChangeUI() =>
		_playerUI.ChangeHealthBar(_health);

	public void ResetHealth()
	{
		_health = _maxHealth;
		ChangeUI();
	}
}
