using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class Interactable_CombinationManager : Interactable, IInputHandler
{
	private bool _isInspecting;
	private bool _canTurn;
	private bool _interacted;
	private bool _done;
	private int[] _digits;

	[SerializeField]
	private Interactable_HingedObjects _lockedObject;
	[SerializeField]
	private CombinationPart[] _combinations;
	[SerializeField]
	private Material _normalMaterial, _selectedMaterial;
	[SerializeField]
	private GameObject _lightSource;
	[SerializeField]
	private string _code;
	[SerializeField]
	private Collider _collider;
	[SerializeField]
	private GameObject _lockCam;
	private GameObject _playerCam;

	private ControlSchema _controls;

	[Inject]
	private GameManager _gameManager;
	[Inject]
	private InputManager _inputManager;

	private int _selectedCombination;

	private void Awake()
	{
		base.Awake();
		_digits = new int[_code.Length];
	}
	public override void OnInteract(Enum_Weapons weapon)
	{
		if (_done) return;

		base.OnInteract(weapon);
		if (_playerCam == null)
			_playerCam = Camera.main.gameObject;

		_interacted = true;
		OnInputEnable(_inputManager.Controls);
		Inspect(true);
	}

	private void Inspect(bool inspect)
	{
		if (!inspect)
			OnInputDisable();
		_canTurn = false;
		if (!_interacted)
			return;

		_playerCam.SetActive(!inspect);
		_lightSource.SetActive(inspect);
		_lockCam.SetActive(inspect);

		_isInspecting = inspect;
		_collider.enabled = !inspect;

		_gameManager.Inspecting = inspect;
		_gameManager.PauseGame = inspect;

	}
	private void CheckCode()
	{
		string code = "";
        foreach (var number in _digits)
			code += number;

		Debug.Log(code);
		if(_code == code)
		{
			Unlock();
		}
    }
	public void ChangeCode(int id, int number)
	{
		_digits[id] = number;
		CheckCode();
	}
	private void StartInspect()
	{
		Inspect(true);
	}
	private void StopInspect()
	{
		Inspect(false);
	}

	private void ChangeCombination(bool next)
	{
		_selectedCombination += next ? 1 : -1;
		_selectedCombination = _selectedCombination < 0 ? _combinations.Length - 1 : _selectedCombination == _combinations.Length ? 0 : _selectedCombination;

		for (int i = 0; i < _combinations.Length; i++)
			_combinations[i].GetComponent<MeshRenderer>().material = _normalMaterial;

		_combinations[_selectedCombination].GetComponent<MeshRenderer>().material = _selectedMaterial;
	}
	private void TurnCombination(InputAction.CallbackContext ctx)
	{
		if (!_canTurn)
		{
			_canTurn = true;
			return;
		}
		_combinations[_selectedCombination].StartTurnCombination();
	}
	private void Unlock()
	{
		_done = true;
		Inspect(false);
		GetComponent<Rigidbody>().isKinematic = false;
		gameObject.layer = 0;
		_lockedObject.Unlock();
	}

	private void StopInspect(InputAction.CallbackContext ctx)
	{
		Inspect(false);
	}
	private void ChangeCombinationRight(InputAction.CallbackContext ctx)
	{
		ChangeCombination(true);
	}
	private void ChangeCombinationLeft(InputAction.CallbackContext ctx)
	{
		ChangeCombination(false);
	}
	public void OnInputEnable(ControlSchema schema)
	{
		_controls = schema;
		_controls.Lock.Back.performed += StopInspect;
		_controls.Lock.NextCombination.performed += ChangeCombinationRight;
		_controls.Lock.PreviousCombination.performed += ChangeCombinationLeft;
		_controls.Lock.Turn.performed += TurnCombination;
	}


	public void OnInputDisable()
	{
		_controls.Lock.Back.performed -= StopInspect;
		_controls.Lock.NextCombination.performed -= ChangeCombinationRight;
		_controls.Lock.PreviousCombination.performed -= ChangeCombinationLeft;
		_controls.Lock.Turn.performed -= TurnCombination;
	}

	public override GameData GetGameData()
	{
		return null;
	}

	public override void LoadData()
	{
	
	}
}
