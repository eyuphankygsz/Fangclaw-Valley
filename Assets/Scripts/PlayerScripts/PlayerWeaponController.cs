using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour, IInputHandler
{
	[SerializeField] private Weapons _currentWeapon;
	[SerializeField] private Transform _weaponHolder;

	private PlayerInteractions _playerInteractions;

	private Dictionary<string, Weapons> _weapons;
	private string[] _weaponNames;
	private int _weaponIndex, _oldWeaponIndex = -1;


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

	private void Start()
	{
		SelectWeapon(0);
	}


	public void ManageGun()
	{
		if (_currentWeapon != null)
		{
			TryMoveGun();
			TryToUse();
		}
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
	private void TryMoveGun()
	{
		_currentWeapon.Move();
	}
	private void TryToUse()
	{
		_currentWeapon.OnAction();
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
		int nextGunIndex = next ? 1 : -1;
		_weaponIndex = (_weaponIndex + nextGunIndex) % _weapons.Count;
		if (_weaponIndex == -1)
			_weaponIndex = _weapons.Count - 1;

		ChangeWeapon();
	}
	public void SelectWeapon(int index)
	{
		if ((_oldWeaponIndex == index && _currentWeapon != null) || index >= _weapons.Count) return;
		_weaponIndex = index;
		ChangeWeapon();
	}
	private void ChangeWeapon()
	{
		_currentWeapon?.OnChanged();
		_currentWeapon?.gameObject.SetActive(false);

		_weaponIndex %= _weapons.Count;
		_oldWeaponIndex = _weaponIndex;

		_currentWeapon = _weapons[_weaponNames[_weaponIndex]];
		_currentWeapon.gameObject.SetActive(true);

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
}
