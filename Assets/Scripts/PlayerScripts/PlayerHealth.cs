using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IInputHandler
{
	[SerializeField]
	private float _health;



	public float Health
	{
		get => _health;
		set
		{
			_health = value;
			//Call PlayerUI
		}
	}

	private void Awake()
	{
		
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
