using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerWeaponController : MonoBehaviour, IInputHandler
{
	[SerializeField] private Weapons _currentWeapon;
	[SerializeField] private Transform _weaponHolder;

	private PlayerInteractions _playerInteractions;

	private Dictionary<string, Weapons> _weapons;

	private int _takenWeaponCount;
	private string[] _weaponNames;
	private int _weaponIndex, _oldWeaponIndex = -1;

	[Inject]
	private WeaponHelpers _weaponHelpers;
	[Inject]
	private InputManager _inputManager;
	[Inject]
	private GameManager _gameManager;

	private bool _onForce, _gamePaused, _externalWeapon;
	private bool _weaponCountInit;

	private void Awake()
	{

		_gameManager.OnPauseGame += OnPause;

		_playerInteractions = GetComponent<PlayerInteractions>();

		_weapons = new Dictionary<string, Weapons>();
		for (int i = 0; i < _weaponHolder.childCount; i++)
			_weapons.Add(_weaponHolder.GetChild(i).GetChild(0).name, _weaponHolder.GetChild(i).GetChild(0).GetComponent<Weapons>());

		_weaponNames = new string[_weapons.Count];
		int nameIndex = 0;
		foreach (var weaponName in _weapons)
			_weaponNames[nameIndex++] = weaponName.Key;
	}

	public void IncreaseWeaponCount()
	{
		_takenWeaponCount++;
	}
	public void ManageGun()
	{
		if (_currentWeapon != null)
			TryMoveGun();
	}

	public void OnForce(bool force)
	{
		_onForce = force;
	}
	private void OnPause(bool pause, bool force)
	{
		if (force)
			return;

		_gamePaused = pause;
	}
	public void StopWeapon(bool stun)
	{
		if (stun)
		{
			_currentWeapon.OnChanged();
			_currentWeapon.gameObject.SetActive(false);
			_currentWeapon = null;
		}
		else
			SelectWeapon(_oldWeaponIndex);
	}
	public void AddWeapon(Enum_Weapons weapon)
	{
		foreach (var item in _weapons)
		{
			if (item.Value.GetWeaponEnum() == weapon)
			{
				item.Value.IsPicked = true;
				break;
			}
		}
	}
	private void TryMoveGun()
	{
		if (!_onForce)
			_currentWeapon.Move();
		else
			_currentWeapon.OnForce();
	}
	private void ChangeGunByKey(InputAction.CallbackContext ctx)
	{
		if (_currentWeapon == null || _gamePaused) return;
		int bindingIndex = ctx.action.GetBindingIndexForControl(ctx.control);
		SelectWeapon(bindingIndex);
	}
	private void ChangeGunByScroll(InputAction.CallbackContext ctx)
	{
		if (_currentWeapon == null || _gamePaused) return;
		SelectWeapon(ctx.ReadValue<float>() < 0);
	}

	private void SelectWeapon(bool next)
	{
		if (_weaponHelpers.StopChange) return;

		int count = GetCurrentWeaponCount();
		_externalWeapon = false;
		int nextGun = (_weaponIndex + (next ? 1 : -1) + count) % count;

		ChangeWeapon(true, nextGun);
	}
	public void SelectWeapon(int index)
	{
		if (_weaponHelpers.StopChange) return;
		if ((_oldWeaponIndex == index && _currentWeapon != null && !_externalWeapon) || index >= _weapons.Count) return;
		_externalWeapon = false;
		int tempIndex = index;
		ChangeWeapon(false, tempIndex);
	}
	public void SelectInstantWeapon(int index)
	{
		_weaponIndex = index;
		_oldWeaponIndex = _weaponIndex;

		_currentWeapon = _weapons[_weaponNames[_weaponIndex]];
		_currentWeapon.OnSelected(_inputManager.Controls);

		_playerInteractions.ChangeCross(_currentWeapon.GetCross());
	}
	public int GetWeaponIndex()
	{
		return _oldWeaponIndex;
	}
	public void SetFreeze(bool freeze)
	{
		foreach (var item in _weapons)
			item.Value.SetFreeze(freeze);
	}
	private void ChangeWeapon(bool errorChange, int tempIndex)
	{
		if (!_weapons[_weaponNames[tempIndex]].IsPicked)
		{
			if (errorChange)
				SelectWeapon(true);

			return;
		}
		_weaponIndex = tempIndex;
		_oldWeaponIndex = _weaponIndex;


		_currentWeapon?.OnChanged();
		_currentWeapon?.gameObject.SetActive(false);


		_currentWeapon = _weapons[_weaponNames[_weaponIndex]];
		_currentWeapon.gameObject.SetActive(true);
		_currentWeapon.OnSelected(_inputManager.Controls);

		_playerInteractions.ChangeCross(_currentWeapon.GetCross());

	}

	private int GetCurrentWeaponCount()
	{
		int count = 0;
		foreach (var item in _weapons)
			if (item.Value.IsPicked)
				count++;

		return count;
	}
	public void OnInputEnable(ControlSchema schema)
	{
		schema.Player.GunKey.performed += ChangeGunByKey;
		schema.Player.GunScroll.performed += ChangeGunByScroll;
	}

	public void OnInputDisable()
	{

	}

	public void EquipExternalWeapon(string name)
	{
		_externalWeapon = true;
		_currentWeapon?.OnChanged();
		_currentWeapon?.gameObject.SetActive(false);

		_currentWeapon = _weapons[name];

		_currentWeapon.gameObject.SetActive(true);
		_currentWeapon.OnSelected(_inputManager.Controls);

		_playerInteractions.ChangeCross(_currentWeapon.GetCross());
	}
}
