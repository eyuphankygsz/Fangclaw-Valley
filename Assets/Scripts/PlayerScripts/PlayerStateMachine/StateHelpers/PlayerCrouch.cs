using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
	[SerializeField]
	private Transform _headTransform;
	[SerializeField]
	private Vector3 _halfExtents;
	[SerializeField]
	private LayerMask _layerMask;
	[SerializeField]
	private float _castDistance = 1.0f;

	public bool CanGetUp()
	{
		bool isHit = Physics.BoxCast(_headTransform.position, _halfExtents, Vector3.up, Quaternion.identity, _castDistance, _layerMask);
		return !isHit;
	}

	private void OnDrawGizmos()
	{
		if (_headTransform == null) return;

		Gizmos.color = Color.red;

		Vector3 endPosition = _headTransform.position + Vector3.up * _castDistance;

		Gizmos.DrawWireCube(_headTransform.position, _halfExtents * 2);
		Gizmos.DrawLine(_headTransform.position, endPosition);
	}
}
