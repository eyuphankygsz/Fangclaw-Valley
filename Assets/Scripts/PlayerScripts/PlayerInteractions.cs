using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInteractions : MonoBehaviour
{
	[SerializeField]
	private float _rayLength;
	[SerializeField]
	private LayerMask _interactableLayers;

	[SerializeField]
	private Transform _camera;

	[SerializeField]
	private Image _cross;
	[SerializeField]
	private Sprite _interactionSprite;
	private Sprite _weaponCross;

	private bool _canInteract;

	private void Awake()
	{
		_camera = Camera.main.transform;
	}
	public void CheckForInteractions()
	{
		if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _rayLength, _interactableLayers))
		{
			SetInteractCross(true);
			if (Input.GetKeyDown(KeyCode.E))
			{
				var _hitObject = hit.collider.gameObject.GetComponent<Interactable>();
				_hitObject.OnInteract(Enum_Weapons.Hands);
			}
		}
		else
			SetInteractCross(false);
	}

	private void SetInteractCross(bool changeCross)
	{

		if (changeCross && !_canInteract)
		{
			_cross.sprite = _interactionSprite;
		}
		else if (!changeCross && _canInteract)
		{
			_cross.sprite = _weaponCross;
		}
		_canInteract = changeCross;
	}
	private void OnDrawGizmos()
	{
		if (_camera == null) _camera = Camera.main.transform;
		Gizmos.color = Color.green;
		Gizmos.DrawRay(_camera.position, _camera.forward * _rayLength);
	}
	public void ChangeCross(Sprite cross)
	{
		_weaponCross = cross;
	}
}
