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
		Debug.Log("Controls Setting...");
		_controls = schema;
		_controls.Player.Interaction.performed += TryInteract;
	}

	public void OnInputDisable()
	{
		_controls.Player.Interaction.performed -= TryInteract;
	}


	public void TryInteract(InputAction.CallbackContext ctx)
	{
		if (ctx.performed && _interactableObject != null)
		{
			var _hitObject = _interactableObject.GetComponent<Interactable>();
			_hitObject.OnInteract(Enum_Weapons.Hands);
		}
	}


	public void CheckForInteractions()
	{
		bool isFound = Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit, _rayLength, _interactableLayers);
		bool obstacleOnTheWay = false;
		if (isFound)
		{
			_obstacleLength = Vector3.Distance(_camera.position, hit.point);
			if (Physics.Raycast(_camera.position, _camera.forward, out RaycastHit hit2, _obstacleLength))
				obstacleOnTheWay = hit.collider.gameObject == hit2.collider.gameObject ? false : true;
		}


		_interactableObject = (!obstacleOnTheWay && isFound) ? hit.collider.gameObject.GetComponent<Interactable>() : null;

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
