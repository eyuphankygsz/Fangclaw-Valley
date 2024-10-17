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
	private PlayerScan _playerScan;

	private PlayerData _data = new PlayerData();


	[Inject]
	private GameManager _gameManager;
	[Inject]
	private SaveManager _saveManager;
	[Inject]
	private InputManager _inputManager;


	private bool _freeze, _force;

	private bool _hiding;

	private void GameFreeze(bool freeze)
	{
		_freeze = freeze;
		_playerInteractions.StopInteractions(freeze);
		_playerScan.SetFreeze(freeze);
		_playerWeapon.SetFreeze(freeze);
	}
	private void Force(bool force)
	{
		_force = force;
		_freeze = force;
		_playerInteractions.StopInteractions(_freeze);
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
		_gameManager.OnForce += Force;
	}

	void Update()
	{
		if (_freeze) return;

		if(_force)
			_playerWeapon.ManageGun();

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
		_playerScan = GetComponent<PlayerScan>();
	}

	public GameData GetSaveFile()
	{
		Vector3 pos = transform.position;
		_data = new PlayerData()
		{
			Name = "Player",
			Position = pos,
			Rotation = transform.rotation,
			SelectedWeapon = _playerWeapon.GetWeaponIndex(),
			MouseSensitivity = _playerCamera.MouseSensitivity
		};
		return _data;
	}

	public void SetLoadFile()
	{
		_data = _saveManager.GetData<PlayerData>("Player");

		if (_data == null)
		{
			Setup(0);
			_playerCamera.SetSensitivity(2);
			_saveManager.AddSaveableObject(gameObject, GetSaveFile());
			return;
		}

		transform.position = _data.Position;
		transform.rotation = _data.Rotation;

		_playerCamera.SetSensitivity(_data.MouseSensitivity);
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
