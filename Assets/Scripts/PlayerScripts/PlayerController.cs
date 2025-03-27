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

	[SerializeField]
	private PlayerHealth _pHealth;
	[SerializeField]
	private PlayerStamina _pStamina; 
	[SerializeField]
	private PlayerGroundCheck _groundCheck;
	[SerializeField]
	private MapCamera _mapCam;
	private PlayerData _data = new PlayerData();


	[Inject]
	private GameManager _gameManager;
	[Inject]
	private SaveManager _saveManager;
	[Inject]
	private InputManager _inputManager;
	[Inject]
	protected WeaponHelpers _weaponHelpers;

	private bool _freeze, _force;

	private bool _hiding, _inspecting;
	public bool Hiding { get { return _hiding; } }

	private void GameFreeze(bool freeze)
	{
		_freeze = freeze;
		_playerInteractions.StopInteractions(freeze);
		_playerScan.SetFreeze(freeze);
		_playerWeapon.SetFreeze(freeze);
	}
	private void Inspecting(bool inspecting)
	{
		_inspecting = inspecting;
		GameFreeze(inspecting);
	}
	private void Pausing(bool pause,  bool force)
	{
		if (force)
			return;

		if(!_inspecting)
			GameFreeze(pause);
	}
	private void Force(bool force)
	{
		_force = force;
		_freeze = force;
		_playerInteractions.StopInteractions(_freeze);
		_playerWeapon.OnForce(force);
	}
	public void SetPos(Transform transform)
	{
		transform.position = transform.position;
		transform.rotation = transform.rotation;
	}
	private void Awake()
	{
		_instance = this;
	}
	private void Start()
	{
		SetLoadFile();
		_gameManager.OnPauseGame += Pausing;
		_gameManager.OnInspecting += Inspecting;
		_gameManager.OnForce += Force;

		_gameManager.Player = gameObject;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.KeypadEnter))
			PlayerPrefs.DeleteAll();
		if(_freeze && _force)
			_playerWeapon.ManageGun();

		if (_freeze) return;

		_mapCam.SetMap(transform.position, transform.rotation.eulerAngles);

		_playerStateMachine.ExecuteState();
		_playerCamera.ManageRotate();
		_playerWeapon.ManageGun();
		_playerInteractions.CheckForInteractions();

		_groundCheck.IsOnGround();
	}

	public static void AddWeapon(Enum_Weapons weapon)
	{
		_instance._playerWeapon.AddWeapon(weapon);
	}
	public void Hide(bool hide)
	{
		_hiding = hide;
		_weaponHelpers.StopChange = hide;
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
			Health = _pHealth.Health,
			Stamina = _pStamina.Stamina
		};
		return _data;
	}

	public void SetLoadFile()
	{
		_data = _saveManager.GetData<PlayerData>("Player");

		if (_data == null)
		{
			Setup(0);
			_pHealth.ResetHealth();
			_pStamina.ResetStamina();
			_saveManager.AddSaveableObject(gameObject, GetSaveFile());
			return;
		}

		transform.position = _data.Position;
		transform.rotation = _data.Rotation;
		_pHealth.SetHealth(_data.Health);
		_pStamina.SetStamina(_data.Stamina);
		Setup(_data.SelectedWeapon);
		
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());
	}
	private void Setup(int selectedWeapon)
	{
		GetPlayerScripts();
		_inputManager.Setup(this);
		_saveManager.AddSaveableObject(gameObject, _data);
		_playerWeapon.SelectInstantWeapon(selectedWeapon);
	}
}
