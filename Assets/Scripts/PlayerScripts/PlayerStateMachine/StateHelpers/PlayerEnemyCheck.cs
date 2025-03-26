using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerEnemyCheck : MonoBehaviour
{
	[SerializeField]
	private LanternHelpers _lanternHelpers;
	[SerializeField]
	private float _rad;
	[SerializeField]
	private LayerMask _enemyLayer;

	private WaitForSeconds _wait = new WaitForSeconds(1);
	private bool _found;

	private Coroutine _checkRoutine;
	private bool _stopEnemyCheck;

	[Inject]
	private PlayerUI _playerUI;
	[Inject]
	private GameManager _manager;

	private void Start()
	{
		_manager.OnChase += OnChase;
	}

	private void OnChase(bool onChase)
	{
		if (onChase)
		{
			TryStopRoutine();
			_checkRoutine = StartCoroutine(CheckEnemies());
		}
		else if (_checkRoutine != null)
		{
			TryStopRoutine();
		}
	}

	private void TryStopRoutine()
	{
		if (_checkRoutine == null) return;

		StopCoroutine(_checkRoutine);
		_found = false;
		_lanternHelpers.StopLightWave();

		_playerUI.StopShake();
	}

	IEnumerator CheckEnemies()
	{
		while (true)
		{
			yield return _wait;

			Collider[] cols = Physics.OverlapSphere(transform.position, _rad, _enemyLayer);
			if (cols.Length != 0)
			{
				if (!_found)
				{
					_found = true;
					_lanternHelpers.StartLightWave();

					_playerUI.SetShakeStrength(1);
					_playerUI.StartShake(0);
				}
			}
			else if (_found)
			{
				_found = false;
				_lanternHelpers.StopLightWave();

				_playerUI.StopShake();
			}
			yield return null;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, _rad);
	}
}
