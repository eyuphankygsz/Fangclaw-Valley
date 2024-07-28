using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public static PlayerController Instance;

	[SerializeField]
	private bool _stopMove;
	public bool StopMove
	{
		get => _stopMove;
		set
		{
			_stopMove = value;
			_playerInteractions.StopInteractions(value);
		}
	}

	private PlayerMovement _playerMovement;
	private PlayerCamera _playerCamera;
	private PlayerWeaponController _playerWeapon;
	private PlayerInteractions _playerInteractions;


	private void Awake()
	{
		Instance = this;
		GetPlayerScripts();
	}
	private void Start()
	{
		Load();
	}
	private void Load() 
	{
		PlayerData data = (PlayerData)SaveManager.Instance.GetData(null,SaveType.Player);
		if (data == null)
			return;

		transform.position = data.Position;
	}

	void Update()
	{
		if (_stopMove) return;
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
}
