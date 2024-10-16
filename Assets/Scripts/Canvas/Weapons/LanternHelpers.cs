using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LanternHelpers : MonoBehaviour
{
	[SerializeField]
	private Image _uiFill;

	private Coroutine _gasRoutine;


	public float MaxFuel { get => _maxFuel; }
	public float LeftFuel;
	public float LitMultiplier = 1;

	private float _leftFuel, _maxFuel = 240f;
	private bool _isOn;

	private void OnEnable()
	{
		if (_isOn)
		{
			if (_gasRoutine != null)
				StopCoroutine(_gasRoutine);
			StartUsingGas();
		}
	}

	public bool IsGasEmpty()
	{
		if (_leftFuel <= 0)
		{
			StopUsingGas();
			_leftFuel = 0;
			ApplyUI();
			return true;
		}
		return false;
	}
	public void StartUsingGas() => _gasRoutine = StartCoroutine(UseGas());
	public void StopUsingGas()
	{
		_isOn = false;
		if (_gasRoutine != null)
			StopCoroutine(_gasRoutine);
	}
	public IEnumerator UseGas()
	{
		_isOn = true;
		while (true)
		{
			_leftFuel -= Time.deltaTime * LitMultiplier;
			ApplyUI();
			yield return null;
		}
	}
	public void AddFuel(float fuel)
	{
		_leftFuel += fuel;
		_leftFuel = _leftFuel > _maxFuel ? _maxFuel : _leftFuel;
		ApplyUI();

	}
	private void ApplyUI()
	{
		_uiFill.fillAmount = Mathf.InverseLerp(0, _maxFuel, _leftFuel);
	}
}
