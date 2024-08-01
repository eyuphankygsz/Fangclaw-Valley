using System;
using UnityEngine;
using Zenject;

public class PlayerController : MonoBehaviour, ISaveable
{

	private PlayerMovement _playerMovement;
	private PlayerCamera _playerCamera;
	private PlayerWeaponController _playerWeapon;
	private PlayerInteractions _playerInteractions;

	private PlayerData _data = new PlayerData();

	[Inject]
	private GameManager _gameManager;
	[Inject]
	private SaveManager _saveManager;
	[Inject]
	private InputManager _inputManager;

	private bool _freeze;


	private void GameFreeze(bool freeze)
	{
		_freeze = freeze;
		_playerInteractions.StopInteractions(freeze);
	}
	private void Start()
	{
		_gameManager.OnPauseGame += GameFreeze;
		_saveManager.AddSaveableObject(gameObject, _data);
		_inputManager.Setup(this);
		GetPlayerScripts();
		Load();
	}
	private void Load()
	{
		PlayerData data = _saveManager.GetData<PlayerData>("");

		if (data == null)
			return;

		transform.position = data.Position;
		transform.rotation = data.Rotation;
	}

	void Update()
	{
		if (_freeze) return;
		_playerMovement.ManageMove();
		_playerCamera.ManageRotate();
		_playerWeapon.ManageGun();
		_playerInteractions.CheckForInteractions();
	}

	private void GetPlayerScripts()
	{
		_playerMovement = GetComponent<PlayerMovement>();
		_playerCamera = GetComponent<PlayerCamera>();
		_playerWeapon = GetComponent<PlayerWeaponController>();
		_playerInteractions = GetComponent<PlayerInteractions>();
	}

	public GameData GetSaveFile()
	{
		Vector3 pos = transform.position;
		_data = new PlayerData()
		{
			Name = "",
			Position = pos,
			Rotation = transform.rotation
		};
		return _data;
	}

	public void SetLoadFile()
	{
		throw new NotImplementedException();
	}
}
