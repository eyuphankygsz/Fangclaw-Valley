using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInteractions : MonoBehaviour, IInputHandler
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
	private TextMeshProUGUI _interactText;


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
			if(value != _stop || _oldInteractable != _interactableObject)
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
		_interactableObject = isFound ? hit.collider.gameObject.GetComponent<Interactable>() : null;
		
		Stop = isFound;
		_oldInteractable = _interactableObject;
	}
	private void SetInteractCross(bool changeCross)
	{
		if (changeCross)
		{
			_cross.sprite = _interactionSprite;
			_interactText.text = _interactableObject.ObjectName;
		}
		else
		{
			_cross.sprite = _weaponCross;
			_interactText.text = "";
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
	public void StopInteractions(bool stop)
	{
		_interactableObject = null;
		_cross.gameObject.SetActive(!stop); 
		_interactText.text = "";
	}

}
