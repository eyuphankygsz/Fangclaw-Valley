using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWithMe : MonoBehaviour
{
	[SerializeField]
	private Transform _footTransform, _playerTransform;
	[SerializeField]
	private float _castDistance;
	[SerializeField]
	private LayerMask _moveLayer;
	[SerializeField]
	private Vector3 _halfExtents;

	private float _hitDistance;
	private bool _hitting;

	private Transform _moveWithTransform;
	private Collider _collider;
	private RaycastHit _hit;


	public void IsOnMovingPlatform()
	{
		if (Physics.BoxCast(_footTransform.position, _halfExtents, -transform.up, out _hit, Quaternion.identity, _castDistance, _moveLayer))
		{
			_collider = _hit.collider;
				if (_moveWithTransform != _collider.transform)
				{
					_moveWithTransform = _collider.transform;
					_playerTransform.parent = _moveWithTransform;
				}
			_hitting = true;
			_hitDistance = _hit.distance;
		}
		else if (_moveWithTransform != null)
		{
			_playerTransform.parent = null;
			_moveWithTransform = null;
		}
		_hitting = false;
	}

	private void OnDrawGizmos()
	{
		if (_footTransform == null) return;
		if (_hitting)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawRay(_footTransform.position, -transform.up * _hitDistance);
			Gizmos.DrawWireCube(_footTransform.position + -transform.up * _hitDistance, _halfExtents * 2);
		}
		else
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawRay(_footTransform.position, -transform.up * _castDistance);
			Gizmos.DrawWireCube(_footTransform.position + -transform.up * _castDistance, _halfExtents * 2);
		}

	}
}
