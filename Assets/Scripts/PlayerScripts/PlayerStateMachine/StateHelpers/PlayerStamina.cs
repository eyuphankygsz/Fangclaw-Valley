using System.Collections;
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

	private Coroutine _currentRoutine;

	public bool Force;

	private void Start()
	{
		_playerUI.SetMaxStamina(_maxStamina);
	}
	public void ChangeStamina(bool increase)
	{
		StopRoutine();
		if (Force)
			return;

		_currentRoutine = increase ? StartCoroutine(IncreaseStaminaRoutine()) : StartCoroutine(DecreaseStaminaRoutine());
	}
	public void SetStamina(float stamina)
	{
		_stamina = stamina;
		ChangeUI();
	}
	public IEnumerator DecreaseStaminaRoutine()
	{

		while (_stamina > 0)
		{
			if (Force)
				yield break;

			_stamina -= Time.deltaTime * 8;
			ChangeUI();

			if (_stamina < 0)
				_stamina = 0;

			yield return null;
		}
		_currentRoutine = null;
	}
	public IEnumerator IncreaseStaminaRoutine()
	{
		while (_stamina < _maxStamina)
		{
			if (Force)
				yield break;

			_stamina += Time.deltaTime * 4f;
			ChangeUI();

			if (_stamina > _maxStamina)
				_stamina = _maxStamina;

			yield return null;
		}
		_currentRoutine = null;
	}
	public void AddStamina(int addedStamina)
	{
		_stamina = Mathf.Clamp(_stamina + addedStamina, 0, _maxStamina);

		CheckStamina();
		ChangeUI();
	}
	private void ChangeUI() =>
		_playerUI.ChangeStaminaBar(_stamina);
	private void StopRoutine()
	{
		if (_currentRoutine != null)
			StopCoroutine(_currentRoutine);
	}
	private void CheckStamina()
	{
		if (_currentRoutine == null)
			if (_stamina < _maxStamina)
				_currentRoutine = StartCoroutine(IncreaseStaminaRoutine());
	}
	public void ResetStamina()
	{
		_stamina = _maxStamina;
		ChangeUI();
	}
}
