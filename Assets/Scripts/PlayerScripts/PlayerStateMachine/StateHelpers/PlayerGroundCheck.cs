using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
	[SerializeField]
	private Transform _footTransform, _playerTransform;
	[SerializeField]
	private float _castDistance;
	[SerializeField]
	private LayerMask _layerMask;
	[SerializeField]
	private Vector3 _halfExtents;

	private float _hitDistance;
	private bool _hitting;

	public bool CantCheckGround;
	private RaycastHit _hit;
	public bool IsOnGround()
	{
		if (CantCheckGround)
			return false;


		if (Physics.BoxCast(_footTransform.position, _halfExtents, -transform.up, out _hit, Quaternion.identity, _castDistance, _layerMask))
		{
			_hitting = true;
			_hitDistance = _hit.distance;
			return true;
		}
		_hitting = false;
		return false;
	}

	private void OnDrawGizmos()
	{
		if (_footTransform == null) return;
		if (_hitting)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(_footTransform.position, -transform.up * _hitDistance);
			Gizmos.DrawWireCube(_footTransform.position + -transform.up * _hitDistance, _halfExtents * 2);
		}
		else
		{
			Gizmos.color = Color.green;
			Gizmos.DrawRay(_footTransform.position, -transform.up * _castDistance);
			Gizmos.DrawWireCube(_footTransform.position + -transform.up * _castDistance, _halfExtents * 2);
		}

	}
}
