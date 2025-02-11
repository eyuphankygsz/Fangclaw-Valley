using FirstGearGames.SmoothCameraShaker;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
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

	[Inject]
	private PlayerUI _playerUI;


	private void Start()
	{
		StartCoroutine(CheckEnemies());
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
					Debug.Log("CHECK FOUND START");

					_playerUI.SetShakeStrength(1);
					_playerUI.StartShake(0);
				}
			}
			else if (_found)
			{
				_found = false;
				_lanternHelpers.StopLightWave();
				Debug.Log("CHECK FOUND STOP");

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
