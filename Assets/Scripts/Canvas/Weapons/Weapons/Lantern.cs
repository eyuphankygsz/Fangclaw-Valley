using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

public class Lantern : Weapons
{
	[SerializeField] private GameObject _normalLightSource;
	[SerializeField] private GameObject _directLightSource;
	[SerializeField] private GameObject _behindLightSource;
	[SerializeField] private InventoryItem _match;
	[SerializeField] private AudioClip _matchSound;
	[SerializeField] private LanternHelpers _lanternHelpers;

	private bool _onFire;


	private Dictionary<int, IWeaponModes> _modes = new Dictionary<int, IWeaponModes>();
	private IWeaponModes _defaultMode;

	private LanternData _data = new LanternData();

	[Inject]
	private InventoryManager _inventoryManager;
	private bool _enlighting;
	public override void Move()
	{
		if (_enlighting) return;
		CheckFuel();
		MoveNormal();
		MoveByCamera();
		ClampMove();
	}

	private void CheckFuel()
	{
		if (_lanternHelpers.IsGasEmpty())
			Delight();
	}


	private void MoveNormal()
	{
		Y_Movement();

		float y = Mathf.InverseLerp(_yLimit.x, _yLimit.y, _pivot.transform.localPosition.y);
		_normalLightSource.transform.localPosition = (Vector3.up * y) + new Vector3(_normalLightSource.transform.localPosition.x, 3f, 0);
		_directLightSource.transform.localPosition = (Vector3.up * y) + new Vector3(_directLightSource.transform.localPosition.x, 3f, 0);
	}
	private void MoveByCamera()
	{
		X_Movement();
		_normalLightSource.transform.localPosition = new Vector3((-_pivot.transform.localPosition.x - 1) * 1.5f, _normalLightSource.transform.localPosition.y, 0);
		_directLightSource.transform.localPosition = new Vector3((-_pivot.transform.localPosition.x - 1) * 1.5f, _directLightSource.transform.localPosition.y, 0);
	}
	private void ClampMove()
	{
		ClampTransform();
	}

	private void OnLeftTrigger(InputAction.CallbackContext ctx)
	{
		if (ctx.performed && !_isFreeze)
			HandleActivation();

	}
	private void OnRightTriggerPerformed(InputAction.CallbackContext ctx)
	{
		if (ctx.performed && !_isFreeze)
		{
			SetLightning(false);
			_lanternHelpers.LitMultiplier = 4;
		}
	}
	private void OnRightTriggerCanceled(InputAction.CallbackContext ctx)
	{
		if (ctx.canceled && !_isFreeze)
		{
			SetLightning(true);
			_lanternHelpers.LitMultiplier = 1;
		}
	}
	private void OnEnable()
	{
		_animator?.SetBool("OnFire", _onFire);
	}
	private void HandleActivation()
	{
		if (_enlighting) return;

		if (!_onFire)
		{
			var item = _inventoryManager.GetItem(_match.ItemName.GetLocalizedString(), 1);

			if (item == null || _lanternHelpers.IsGasEmpty())
				return;

			_lanternHelpers.StartUsingGas();
			_enlighting = true;
			_weaponHelpers.StopChange = true;
			_source.clip = _matchSound;
			_source.Play();
			_animator.SetTrigger("Enlight");
			_inventoryManager.RemoveItemQuantityFromInventory(item, 1);
		}
		else
		{
			_lanternHelpers.StopUsingGas();

			_enlighting = true;
			_animator.SetTrigger("Delight");
		}
	}
	public void Enlight()
	{
		_onFire = true;
		_animator.SetBool("OnFire", _onFire);
		_lanternHelpers.StartUsingGas();
		_enlighting = false;
		_weaponHelpers.StopChange = false;
		SetLightning(true);
	}
	public void Delight()
	{
		_onFire = false;
		_animator.SetBool("OnFire", _onFire);
		_enlighting = false;
		SetLightning(false);
	}


	private void SetLightning(bool isDefault)
	{
		if (_onFire)
		{
			_normalLightSource.SetActive(isDefault);
			_directLightSource.SetActive(!isDefault);
		}
		else
		{
			_normalLightSource.SetActive(false);
			_directLightSource.SetActive(false);
		}
	}

	public override void OnSelected(ControlSchema schema)
	{
		_controls = schema;
		_controls.Player.PrimaryShoot.performed += OnLeftTrigger;
		_controls.Player.SecondaryShoot.performed += OnRightTriggerPerformed;
		_controls.Player.SecondaryShoot.canceled += OnRightTriggerCanceled;

		_animator.SetBool("OnFire", _onFire);
		_normalLightSource.SetActive(_onFire);
		_behindLightSource.SetActive(false);
	}

	public override void OnChanged()
	{
		if (_controls != null)
		{
			_controls.Player.PrimaryShoot.performed -= OnLeftTrigger;
			_controls.Player.SecondaryShoot.performed -= OnRightTriggerPerformed;
			_controls.Player.SecondaryShoot.canceled -= OnRightTriggerCanceled;
		}

		_directLightSource.SetActive(false);
		_normalLightSource.SetActive(false);
		_behindLightSource.SetActive(_onFire);
	}
	public override void SetWeapon()
	{
		_defaultMode = new DefaultMode();

		_modes.Add(9, new EnemyHitMode()); //EnemyHits

		foreach (var wMode in _modes)
			wMode.Value.Setup(this);
	}

	public override GameData GetSave()
	{
		return new LanternData()
		{
			Name = gameObject.name,
			LeftFuel = _lanternHelpers.LeftFuel,
			OnFire = _onFire,
			IsSelected = gameObject.activeSelf,

			IsPicked = IsPicked
		};
	}

	public override void LoadSave()
	{
		LanternData data = _saveManager.GetData<LanternData>(gameObject.name);
		if (data == null)
		{
			gameObject.SetActive(false);
			return;
		}

		IsPicked = data.IsPicked;
		_lanternHelpers.LeftFuel = data.LeftFuel;
		_data = data;

		if (_data.OnFire)
			Enlight();

		gameObject.SetActive(data.IsSelected);
	}
}
