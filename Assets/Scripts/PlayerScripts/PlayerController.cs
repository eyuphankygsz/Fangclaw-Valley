using System;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour, ISaveable
{
	private static PlayerController _instance;

	private PlayerCamera _playerCamera;
	private PlayerWeaponController _playerWeapon;
	private PlayerInteractions _playerInteractions;
	private PlayerStateMachine _playerStateMachine;

	private PlayerData _data = new PlayerData();


	[Inject]
	private GameManager _gameManager;
	[Inject]
	private SaveManager _saveManager;
	[Inject]
	private InputManager _inputManager;


	private bool _freeze;

	private bool _hiding;

	private void GameFreeze(bool freeze)
	{
		_freeze = freeze;
		_playerInteractions.StopInteractions(freeze);
	}
	private void Awake()
	{
		_instance = this;
	}
	private void Start()
	{
		SetLoadFile();
		_gameManager.OnPauseGame += GameFreeze;
		_gameManager.OnInspecting += GameFreeze;
	}

	void Update()
	{
		if (_freeze) return;
		_playerStateMachine.ExecuteState();
		_playerCamera.ManageRotate();
		_playerWeapon.ManageGun();
		_playerInteractions.CheckForInteractions();
	}

	public static void AddWeapon(Enum_Weapons weapon)
	{
		_instance._playerWeapon.AddWeapon(weapon);
	}
	public void Hide(bool hide)
	{
		if (_hiding == hide) return;
		_hiding = hide;

		_playerWeapon.StopWeapon(_hiding);
	}
	private void GetPlayerScripts()
	{
		_playerStateMachine = GetComponent<PlayerStateMachine>();
		_playerCamera = GetComponent<PlayerCamera>();
		_playerWeapon = GetComponent<PlayerWeaponController>();
		_playerInteractions = GetComponent<PlayerInteractions>();
	}

	public GameData GetSaveFile()
	{
		Vector3 pos = transform.position;
		_data = new PlayerData()
		{
			Name = "Player",
			Position = pos,
			Rotation = transform.rotation,
			SelectedWeapon = _playerWeapon.GetWeaponIndex()
		};
		return _data;
	}

	public void SetLoadFile()
	{
		_data = _saveManager.GetData<PlayerData>("Player");

		if (_data == null)
		{
			Setup(0);
			_saveManager.AddSaveableObject(gameObject, GetSaveFile());
			return;
		}

		transform.position = _data.Position;
		transform.rotation = _data.Rotation;
		Setup(_data.SelectedWeapon);
		
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
	}
	private void Setup(int selectedWeapon)
	{
		GetPlayerScripts();
		_inputManager.Setup(this);
		_saveManager.AddSaveableObject(gameObject, _data);
		_playerWeapon.SelectWeapon(selectedWeapon);
	}
}
