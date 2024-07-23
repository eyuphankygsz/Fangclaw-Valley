using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
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
			TryToChangeGun();
		}
	}
	private void TryMoveGun()
	{
		_currentWeapon.Move();
	}
	private void TryToUse()
	{
		_currentWeapon.OnAction();
	}
	private void TryToChangeGun()
	{
		if (Input.inputString != null)
		{
			bool isNumber = int.TryParse(Input.inputString, out int number);
			if (isNumber && number > 0 && number < 4)
			{
				SelectWeapon(number - 1);
			}
			else
			{
				if (Input.GetAxis("Mouse ScrollWheel") > 0f)
					SelectWeapon(true);
				else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
					SelectWeapon(false);
			}
		}
	}
	private void SelectWeapon(bool next)
	{
		_weaponIndex += next ? 1 : (_weaponIndex - 1 < 0 ? _weapons.Count - 1 : _weaponIndex - 1);
		ChangeWeapon();
	}
	public void SelectWeapon(int index)
	{
		Debug.Log("OLD: " + _oldWeaponIndex + " INPUT: " + index);
		if (_oldWeaponIndex == index) return;
		_weaponIndex = index;
		ChangeWeapon();
	}
	private void ChangeWeapon()
	{
		_currentWeapon.OnChanged();
		_currentWeapon.gameObject.SetActive(false);

		_weaponIndex %= _weapons.Count;
		_oldWeaponIndex = _weaponIndex;

		_currentWeapon = _weapons[_weaponNames[_weaponIndex]];
		_currentWeapon.gameObject.SetActive(true);

		_playerInteractions.ChangeCross(_currentWeapon.GetCross());

	}


}
