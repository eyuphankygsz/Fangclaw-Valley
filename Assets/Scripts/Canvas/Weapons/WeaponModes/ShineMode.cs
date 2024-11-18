using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ShineMode : MonoBehaviour
{
	public Weapons Weapon { get; set; }
	[SerializeField]
	private Transform _center;
	[SerializeField]
	private Vector3 _halfExtents;
	[SerializeField]
	private float _range;
	[SerializeField]
	private LayerMask _enemyLayer, _seeThrough;

	private bool _hitBool;
	RaycastHit _hit;
	public void ExecuteModeUpdate()
	{
		RaycastHit hit;
		if (Physics.SphereCast(_center.position, _halfExtents.x, _center.forward, out hit, _range, _enemyLayer))
		{
			RaycastHit hitOthers;
			if (Physics.SphereCast(_center.position, _halfExtents.x, _center.forward, out hitOthers))
			{
				int layer = hitOthers.collider.gameObject.layer;
				if ((_seeThrough.value & (1 << layer)) != 0)
				{
					if (hit.transform.TryGetComponent<WhispererController>(out WhispererController controller))
					{
						_hit = hit;
						controller.Shined();
						_hitBool = true;
					}
					else
						_hitBool = false;
				}
			}
		}
		else
			_hitBool = false;
	}
	private void OnDrawGizmos()
	{
		if (!gameObject.activeSelf) return;

		Gizmos.color = _hitBool ? Color.yellow : Color.red;

		Vector3 endPoint = _hitBool ? _hit.point : _center.position + (_center.forward * _range);

		Gizmos.DrawLine(_center.position, endPoint);
		Gizmos.DrawWireSphere(endPoint, _halfExtents.x);

	}
}
