using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
	[SerializeField]
	private float _rayLength;
	[SerializeField]
	private LayerMask _interactableLayers;

	[SerializeField]
	private Transform _camera;
	private void Awake()
	{
		_camera = Camera.main.transform;
	}
	public void CheckForInteractions()
	{
		if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _rayLength, _interactableLayers))
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				var _hitObject = hit.collider.gameObject.GetComponent<Interactable>();
				_hitObject.OnInteract(Enum_Weapons.Hands);
			}
		}
	}
	private void OnDrawGizmos()
	{
		if (_camera == null) _camera = Camera.main.transform;
		Gizmos.color = Color.green;
		Gizmos.DrawRay(_camera.position, _camera.forward * _rayLength);
	}
}
