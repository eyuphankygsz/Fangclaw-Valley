using System.Collections;
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
	private int _pickedWeaponCount; // Cached count of picked weapons

	[Inject]
	private WeaponHelpers _weaponHelpers;
	[Inject]
	private InputManager _inputManager;
	[Inject]
	private GameManager _gameManager;

	private bool _onForce, _gamePaused, _externalWeapon;
	private bool _weaponCountInit;

	public bool OnWeaponChanging;
	private void Awake()
	{
		_gameManager.OnPauseGame += OnPause;
		_gameManager.OnWeaponChanging += OnWeaponChange;

		_playerInteractions = GetComponent<PlayerInteractions>();

		_weapons = new Dictionary<string, Weapons>();
		for (int i = 0; i < _weaponHolder.childCount; i++)
		{
			var weapon = _weaponHolder.GetChild(i).GetChild(0).GetComponent<Weapons>();
			_weapons.Add(weapon.name, weapon);
		}

		_weaponNames = new string[_weapons.Count];
		_weapons.Keys.CopyTo(_weaponNames, 0);
		UpdatePickedWeaponCount(); // Initialize the count
	}

	private void Start()
	{
		_weaponHelpers.SetWeaponController(this);
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
		if (!force) _gamePaused = pause;
	}
	private void OnWeaponChange(bool change)
	{
		OnWeaponChanging = change;
	}
	public void StopWeapon(bool stun)
	{
		if (stun)
		{
			_currentWeapon.OnChanged();
			_currentWeapon = null;
		}
		else
			SelectInstantWeapon(_oldWeaponIndex);
	}
	public void AddWeapon(Enum_Weapons weapon)
	{
		foreach (var item in _weapons)
		{
			if (item.Value.GetWeaponEnum() == weapon)
			{
				item.Value.IsPicked = true;
				UpdatePickedWeaponCount(); // Update count when a weapon is added
				break;
			}
		}
	}
	private void TryMoveGun()
	{
		if (!_onForce && !OnWeaponChanging)
			_currentWeapon.Move();
		else if (_onForce)
			_currentWeapon.OnForce();
	}
	private void ChangeGunByKey(InputAction.CallbackContext ctx)
	{
		if (_currentWeapon == null || _gamePaused || OnWeaponChanging) return;
		int bindingIndex = ctx.action.GetBindingIndexForControl(ctx.control);
		OnWeaponChanging = true;
		SelectWeapon(bindingIndex);
	}
	private void ChangeGunByScroll(InputAction.CallbackContext ctx)
	{
		if (_currentWeapon == null || _gamePaused || OnWeaponChanging) return;
		OnWeaponChanging = true;
		SelectWeapon(ctx.ReadValue<float>() < 0);
	}

	private void SelectWeapon(bool next)
	{
		int nextGun = (_weaponIndex + (next ? 1 : -1) + _pickedWeaponCount) % _pickedWeaponCount;

		if (_weaponHelpers.StopChange || _weaponIndex == nextGun)
		{
			OnWeaponChanging = false;
			return;
		}
		_externalWeapon = false;

		ChangeWeapon(true, nextGun);
	}
	public void SelectWeapon(int index)
	{
		if ((_oldWeaponIndex == index && _currentWeapon != null && !_externalWeapon) || 
			index >= _weapons.Count || 
			_weaponHelpers.StopChange || 
			_weaponIndex == index)
		{
			OnWeaponChanging = false;
			return;
		}
		_externalWeapon = false;
		ChangeWeapon(false, index);
	}
	public void SelectInstantWeapon(int index)
	{
		_currentWeapon?.OnChanged();
		_weaponIndex = index;
		_oldWeaponIndex = _weaponIndex;

		if (_weapons.TryGetValue(_weaponNames[_weaponIndex], out _currentWeapon))
		{
			StartCoroutine(WaitForWeaponAnimation(false));
		}
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
		if (!_weapons.TryGetValue(_weaponNames[tempIndex], out var weapon) || !weapon.IsPicked)
		{
			OnWeaponChanging = false;
			if (errorChange)
				SelectWeapon(true);
			return;
		}

		bool sameGun = tempIndex == _oldWeaponIndex;

		_weaponIndex = tempIndex;
		_oldWeaponIndex = _weaponIndex;

		_currentWeapon?.OnChanged();
		StartCoroutine(WaitForWeaponAnimation(sameGun));
	}
	private IEnumerator WaitForWeaponAnimation(bool sameGun)
	{
		yield return new WaitForEndOfFrame();
		while (_currentWeapon == null || !_currentWeapon.CanChange)
		{
			yield return null;
		}
		StartCoroutine(CheckWeaponChange(sameGun));
	}

	private IEnumerator CheckWeaponChange(bool sameGun)
	{
		while (_currentWeapon == null)
		{
			yield return null;
		}

		_currentWeapon = _weapons[_weaponNames[_weaponIndex]];
		_currentWeapon.gameObject.SetActive(true);
		_currentWeapon.OnSelected(_inputManager.Controls);
		_playerInteractions.ChangeCross(_currentWeapon.GetCross());
	}

	private void UpdatePickedWeaponCount()
	{
		_pickedWeaponCount = 0;
		foreach (var item in _weapons)
		{
			if (item.Value.IsPicked) _pickedWeaponCount++;
		}
	}
	public void OnInputEnable(ControlSchema schema)
	{
		schema.Player.GunKey.performed += ChangeGunByKey;
		schema.Player.GunScroll.performed += ChangeGunByScroll;
	}

	public void OnInputDisable()
	{

	}
	public Enum_Weapons GetCurrentWeaponEnum() => _currentWeapon.GetWeaponEnum();

	public void EquipExternalWeapon(string name)
	{
		_externalWeapon = true;

		int i = System.Array.IndexOf(_weaponNames, name);
		if (i >= 0)
		{
			bool sameGun = i == _oldWeaponIndex;
			_weaponIndex = i;
			_oldWeaponIndex = _weaponIndex;

			_currentWeapon?.OnChanged();
			StartCoroutine(WaitForWeaponAnimation(sameGun));
		}
	}
	public void TryDropExternalWeapon(string name)
	{
		if (_externalWeapon && _weapons.TryGetValue(name, out Weapons wp) && _currentWeapon == wp)
		{
			_currentWeapon?.gameObject.SetActive(false);
			_currentWeapon = _weapons[_weaponNames[_weaponIndex]];
			SelectWeapon(true);
		}
	}
}
