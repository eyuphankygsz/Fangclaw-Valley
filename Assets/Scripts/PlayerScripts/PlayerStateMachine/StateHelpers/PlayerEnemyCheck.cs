using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
				}
			}
			else if (_found)
			{
				_found = false;
				_lanternHelpers.StopLightWave();
			}
			yield return null;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position,_rad);
	}
}
