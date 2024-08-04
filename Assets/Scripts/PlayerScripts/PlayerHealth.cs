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
	public void AddHealth(float damage)
	{
		_health -= damage;
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
