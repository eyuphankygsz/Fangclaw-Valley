using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class Interactable_CombinationManager : Interactable, IInputHandler
{
	private bool _isInspecting;
	private bool _canTurn;
	private bool _interacted;
	private bool _unlocked;
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
	private Camera _playerCam, _uiCam;
	private ControlSchema _controls;

	[Inject]
	private GameManager _gameManager;
	[Inject]
	private InputManager _inputManager;

	private int _selectedCombination;

	private CombLockData _data;

	private bool _inputsEnabled;

	[SerializeField]
	private AchievementCheck _theKeyHolder;
	[SerializeField]
	private SteamAchievements _achievements;

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	private void Awake()
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
	{
		base.Awake();
		_digits = new int[_code.Length];

	}

	public override void OnInteract(Enum_Weapons weapon)
	{
		if (_unlocked) return;
		base.OnInteract(weapon);
		if (_playerCam == null)
		{
			_playerCam = Camera.main;
			_uiCam = _playerCam.transform.GetChild(0).GetComponent<Camera>();
		}

		_interactEvents?.Invoke();

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

		_playerCam.enabled = _uiCam.enabled = !inspect;

		_lightSource.SetActive(inspect);
		_lockCam.SetActive(inspect);

		_isInspecting = inspect;
		_collider.enabled = !inspect;

		_gameManager.Inspecting = inspect;

	}
	private void CheckCode()
	{
		foreach (var item in _combinations)
			if (item.IsTurning)
				return;

		string code = "";
		foreach (var number in _digits)
			code += number;

		if (_code == code)
		{
			Unlock(false);
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
		if (ctx.performed)
		{
			if (!_canTurn)
			{
				_canTurn = true;
				return;
			}
			_combinations[_selectedCombination].StartTurnCombination(times: 1, setManually: false);
		}
	}
	private void Unlock(bool silent)
	{
		if (!silent)
			_achievements.TryEnableAchievement(_theKeyHolder);

		PlayerPrefs.SetInt("locks_unlocked", PlayerPrefs.GetInt("locks_unlocked") + 1);

		if (!_unlocked)
			_oneTimeEvents?.Invoke();

		_unlocked = true;
		Inspect(false);
		GetComponent<Rigidbody>().isKinematic = false;
		gameObject.layer = 0;
		_lockedObject.Unlock(silent);
	}

	private void StopInspect(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
		{
			if (!_canTurn)
			{
				_canTurn = true;
				return;
			}
			Inspect(false);
		}
	}
	private void ChangeCombinationRight(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
			ChangeCombination(true);
	}
	private void ChangeCombinationLeft(InputAction.CallbackContext ctx)
	{
		if (ctx.performed)
			ChangeCombination(false);
	}
	public void OnInputEnable(ControlSchema schema)
	{
		_controls = schema;
		_controls.Player.Back.performed += StopInspect;
		_controls.Player.NextCombination.performed += ChangeCombinationRight;
		_controls.Player.PreviousCombination.performed += ChangeCombinationLeft;
		_controls.Player.Turn.performed += TurnCombination;

		_inputsEnabled = true;
	}


	public void OnInputDisable()
	{
		if (!_inputsEnabled) return;
		_controls.Player.NextCombination.performed -= ChangeCombinationRight;
		_controls.Player.PreviousCombination.performed -= ChangeCombinationLeft;
		_controls.Player.Back.performed -= StopInspect;
		_controls.Player.Turn.performed -= TurnCombination;
		_inputsEnabled = false;
	}

	public override GameData GetGameData()
	{
		if (_digits == null) _digits = new int[_code.Length];
		_data = new CombLockData()
		{
			Name = InteractableName,
			IsUnlocked = _unlocked,
			LastIDList = _digits.ToList()
		};
		return _data;
	}

	public override void LoadData()
	{
		CombLockData data = _saveManager.GetData<CombLockData>(InteractableName);
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
		if (data == null) return;

		if (data.IsUnlocked)
		{
			_unlocked = true;
			gameObject.SetActive(false);
			return;
		}

		_digits = data.LastIDList.ToArray();
		for (int i = 0; i < _combinations.Length; i++)
		{
			int tempID = 0;
			int turnCount = -1;
			int max = _combinations[i].GetMaxNumber();
			while (tempID != _digits[i])
			{
				tempID = (tempID + 1) % max;
				turnCount++;
			}
			_combinations[i].StartTurnCombination(times: turnCount, setManually: true);
			_combinations[i].Loaded = true;
		}
	}
}
