using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteractions : MonoBehaviour, IInputHandler
{
	[SerializeField]
	private float _rayLength;
	[SerializeField]
	private float _obstacleLength;
	[SerializeField]
	private LayerMask _interactableLayers;
	[SerializeField]
	private LayerMask _excludeLayers;

	[SerializeField]
	private Transform _camera;

	[SerializeField]
	private Image _cross;
	[SerializeField]
	private TextMeshProUGUI _interactText;

	private string _lastInteractName;


	[SerializeField]
	private Sprite _interactionSprite;
	private Sprite _weaponCross;
	private bool _canInteract;

	private Interactable _interactableObject;
	private Interactable _oldInteractable;
	private ControlSchema _controls;

	private bool _stop;

	private bool Stop
	{
		get => _stop;
		set
		{
			if (value != _stop || _oldInteractable != _interactableObject)
			{
				SetInteractCross(value);
			}
			_stop = value;
		}
	}

	private void Awake()
	{
		_camera = Camera.main.transform;
	}

	public void OnInputEnable(ControlSchema schema)
	{
		_controls = schema;
		_controls.Player.Interaction.performed += TryInteract;
		_controls.Player.Interaction.canceled += TryStopInteract;
	}

	public void OnInputDisable()
	{
		_controls.Player.Interaction.performed -= TryInteract;
		_controls.Player.Interaction.canceled -= TryStopInteract;
	}

	private Interactable _currentInteractable;
	public void TryInteract(InputAction.CallbackContext ctx)
	{
		if (ctx.performed && _interactableObject != null)
		{
			_currentInteractable = _interactableObject.GetComponent<Interactable>();
			_currentInteractable.OnInteract(Enum_Weapons.Hands);
		}
	}
	public void TryStopInteract(InputAction.CallbackContext ctx)
	{
		if (ctx.canceled && _interactableObject != null)
		{
			_currentInteractable = _interactableObject.GetComponent<Interactable>();
			_currentInteractable.OnStopInteract(Enum_Weapons.Hands);
		}
	}

	public void CheckForInteractions()
	{
		// Kamera y�n�nde ilk raycast
		bool isFound = Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _rayLength, _interactableLayers);
		bool obstacleOnTheWay = false;

		if (isFound)
		{
			_obstacleLength = Vector3.Distance(_camera.position, hit.point);

			RaycastHit[] hittedObjects = Physics.RaycastAll(_camera.position, _camera.forward, _obstacleLength);

			foreach (var item in hittedObjects)
			{
				int itemLayer = item.transform.gameObject.layer;

				if ((_excludeLayers.value & (1 << itemLayer)) != 0 || item.collider.gameObject == hit.collider.gameObject)
					obstacleOnTheWay = false;
				else
				{
					obstacleOnTheWay = true;
					break;
				}
			}
		}

		_interactableObject = (!obstacleOnTheWay && isFound) ? hit.collider.gameObject.GetComponent<Interactable>() : null;

		if(_oldInteractable != null && _interactableObject == null)
			_oldInteractable.OnStopInteract(Enum_Weapons.Hands);

		Stop = !obstacleOnTheWay && isFound;
		_oldInteractable = _interactableObject;
	}
	private void SetInteractCross(bool changeCross)
	{
		if (changeCross)
		{
			_cross.sprite = _interactionSprite;
			_interactText.text = _interactableObject.ObjectName;
			_lastInteractName = _interactText.text;
		}
		else
		{
			_cross.sprite = _weaponCross;
			_interactText.text = "";
			_lastInteractName = "";
		}

		_canInteract = changeCross;
	}
	private void OnDrawGizmos()
	{
		if (_camera == null) _camera = Camera.main.transform;
		Gizmos.color = Color.green;
		Gizmos.DrawRay(_camera.position, _camera.forward * _obstacleLength);
	}
	public void ChangeCross(Sprite cross)
	{
		_weaponCross = cross;
	}
	public void StopInteractions(bool stop)
	{
		_cross.gameObject.SetActive(!stop);
		_interactableObject = stop ? null : _interactableObject;
		_interactText.text = stop ? "" : _lastInteractName;
	}

}
