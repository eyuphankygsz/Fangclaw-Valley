using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerHealth : MonoBehaviour, IInputHandler
{
	[SerializeField]
	private float _health, _maxHealth;

	[Inject]
	private PlayerUI _playerUI;

	public float Health
	{
		get => _health;
		set
		{
			_health = value;
		}
	}

	private void Awake()
	{
		_playerUI.SetMaxHealth(_maxHealth);
	}
	public void AddHealth(float addedHealth)
	{
		_health = Mathf.Clamp(_health + addedHealth, 0, _maxHealth);
		_playerUI.ChangeHealthBar(_health);
	}
	public void OnInputDisable()
	{

	}

	public void OnInputEnable(ControlSchema schema)
	{

	}
	public void Run()
	{

	}
}
