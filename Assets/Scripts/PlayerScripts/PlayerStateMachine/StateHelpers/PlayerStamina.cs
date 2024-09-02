using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerStamina : MonoBehaviour
{
	[SerializeField] 
	private float _maxStamina;

	private float _stamina;
    public float Stamina { get { return _stamina; } }

	[Inject]
	private PlayerUI _playerUI;

	private void Awake()
	{
		_stamina = _maxStamina;
	}
	private void Start()
	{
		_playerUI.SetMaxStamina(_maxStamina);
	}
	public IEnumerator DecreaseStamina()
	{

		while (_stamina > 0)
		{
			_stamina -= Time.deltaTime;
			_playerUI.ChangeStaminaBar(_stamina);
			
			if(_stamina < 0)
				_stamina = 0;
			
			yield return null;
		}
	}

	public IEnumerator IncreaseStamina()
	{
		while (_stamina < _maxStamina)
		{
			_stamina += Time.deltaTime * 0.2f;
			_playerUI.ChangeStaminaBar(_stamina);

			if (_stamina > _maxStamina)
				_stamina = _maxStamina;
			
			yield return null;
		}
	}
}
