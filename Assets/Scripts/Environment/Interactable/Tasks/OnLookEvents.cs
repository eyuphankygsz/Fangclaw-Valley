using UnityEngine;
using UnityEngine.Events;

public class OnLookEvents : MonoBehaviour
{
	[SerializeField]
	private UnityEvent _events;

	public UnityEvent ForceEvents;

	private bool _hasTriggered;

	[SerializeField]
	private Camera _mainCamera;
	[SerializeField]
	private Transform _targetObject;

	[SerializeField]
	private float _maxAngle = 30f;
	[SerializeField]
	private float _maxDistance = 5f;
	[SerializeField]
	private LayerMask _layerMask;

	private void Start()
	{
		_mainCamera = Camera.main;
	}

	public void Update()
	{
		if (_hasTriggered) return;

		Vector3 directionToTarget = _targetObject.position - _mainCamera.transform.position;
		float angleToTarget = Vector3.Angle(_mainCamera.transform.forward, directionToTarget);


		if (angleToTarget <= _maxAngle && !IsObstructed(directionToTarget))
		{
			_events?.Invoke();
			_hasTriggered = true;
		}
	}

	bool IsObstructed(Vector3 directionToTarget)
	{
		float distanceToTarget = directionToTarget.magnitude;

		if (distanceToTarget > _maxDistance)
			return true;

		Ray ray = new Ray(_mainCamera.transform.position, directionToTarget.normalized);
		RaycastHit hit;

		if (Physics.Raycast(ray, out hit, distanceToTarget,_layerMask))
			if (hit.transform != _targetObject)
				return true;

		return false;
	}

	public void Restart() { }
	private void OnDrawGizmos()
	{
		if (_targetObject != null || _mainCamera != null)
		{
			Gizmos.color = Color.red;
			Vector3 directionToTarget = _targetObject.position - _mainCamera.transform.position;

			Vector3 forwardDirection = _mainCamera.transform.forward;

			Quaternion leftAngle = Quaternion.Euler(0, -_maxAngle, 0);
			Quaternion rightAngle = Quaternion.Euler(0, _maxAngle, 0);
			Gizmos.DrawLine(_mainCamera.transform.position, _mainCamera.transform.position + forwardDirection * _maxDistance); // Ön
			Gizmos.DrawLine(_mainCamera.transform.position, _mainCamera.transform.position + leftAngle * forwardDirection * _maxDistance); // Sol
			Gizmos.DrawLine(_mainCamera.transform.position, _mainCamera.transform.position + rightAngle * forwardDirection * _maxDistance); // Sað

			Gizmos.color = Color.green;
			Gizmos.DrawSphere(_targetObject.position, 0.1f);
		}
	}
}
