using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockHand : MonoBehaviour
{
	[SerializeField]
	private bool _minuteHand;
	[SerializeField]
	private Transform _center;
	[SerializeField]
	private Vector3 _halfExt;

	[SerializeField]
	private LayerMask _layer;

	public int GetTime()
	{
		Collider[] cols = Physics.OverlapBox(_center.position,_halfExt,Quaternion.identity, _layer);
		if (cols.Length > 0)
		{
			return cols[0].GetComponent<ClockSet>().Minute;
		}

		return -1;
	}
	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(_center.position, _halfExt / 2);
	}
}
