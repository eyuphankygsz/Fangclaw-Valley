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
	private string[] _weaponNames;
	private int _weaponIndex, _oldWeaponIndex = -1;

	[Inject]
	private WeaponHelpers _weaponHelpers;
	[Inject]
	private InputManager _inputManager;
	private void Awake()
	{
		_playerInteractions = GetComponent<PlayerInteractions>();

		_weapons = new Dictionary<string, Weapons>();
		for (int i = 0; i < _weaponHolder.childCount; i++)
			_weapons.Add(_weaponHolder.GetChild(i).GetChild(0).name, _weaponHolder.GetChild(i).GetChild(0).GetComponent<Weapons>());

		_weaponNames = new string[_weapons.Count];
		int nameIndex = 0;
		foreach (var weaponName in _weapons)
			_weaponNames[nameIndex++] = weaponName.Key;
	}


	public void ManageGun()
	{
		if (_currentWeapon != null)
			TryMoveGun();
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
		_currentWeapon.Move();
	}
	private void ChangeGunByKey(InputAction.CallbackContext ctx)
	{
		if (_currentWeapon == null) return;
		int bindingIndex = ctx.action.GetBindingIndexForControl(ctx.control);
		SelectWeapon(bindingIndex);
	}
	private void ChangeGunByScroll(InputAction.CallbackContext ctx)
	{
		if (_currentWeapon == null) return;
		SelectWeapon(ctx.ReadValue<float>() < 0);
	}

	private void SelectWeapon(bool next)
	{
		if (_weaponHelpers.StopChange) return;
		int nextGunIndex = next ? 1 : -1;
		int tempIndex = (_weaponIndex + nextGunIndex) % _weapons.Count;
		_weaponIndex++;
		if (tempIndex == -1)
			tempIndex = _weapons.Count - 1;

		ChangeWeapon(true, tempIndex);
	}
	public void SelectWeapon(int index)
	{
		if (_weaponHelpers.StopChange) return;
		if ((_oldWeaponIndex == index && _currentWeapon != null) || index >= _weapons.Count) return;
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
		_currentWeapon?.OnChanged();
		_currentWeapon?.gameObject.SetActive(false);

		_currentWeapon = _weapons[name];

		_currentWeapon.gameObject.SetActive(true);
		_currentWeapon.OnSelected(_inputManager.Controls);

		_playerInteractions.ChangeCross(_currentWeapon.GetCross());
	}
}
