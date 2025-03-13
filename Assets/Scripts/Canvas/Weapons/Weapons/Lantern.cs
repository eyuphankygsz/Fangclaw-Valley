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
	[SerializeField] private Light _onHandLight;



	[SerializeField] private GameObject _playerLight;
	[SerializeField] private InventoryItem _match;
	[SerializeField] private AudioClip _matchSound, _gasSound;
	[SerializeField] private LanternHelpers _lanternHelpers;
	[SerializeField] private ShineMode _shine;
	[SerializeField] private AudioClip _switchIn, _switchOut;
	[SerializeField] private AudioSource _gasLeakLoop;
	private bool _onFire;
	private bool _isShining;
	public bool IsShining { get { return _isShining; } }
	private bool _isOnHand;

	private Dictionary<int, IWeaponModes> _modes = new Dictionary<int, IWeaponModes>();
	private IWeaponModes _defaultMode;

	private LanternData _data = new LanternData();

	[Inject]
	private InventoryManager _inventoryManager;
	private bool _enlighting;



	private float _initialIntensityNormal = 4, _initialIntensityDirect = 6, _initialIntensityBehind = 2;
	private Light[] _lanternLights;



	private Vector3 _latestPos;
	public override void Move()
	{
		if (_enlighting) return;
		SaveLastPos();
		CheckFuel();
		MoveNormal();
		MoveByCamera();
		ClampMove();
		HandleShine();
	}

	private void SaveLastPos()
	{
		if (_weaponHelpers.StopChange) return;
		_latestPos = transform.localPosition;
	}
	private void CheckFuel()
	{
		if (_lanternHelpers.IsGasEmpty())
			Delight();
	}
	private void HandleShine()
	{
		if (_isShining)
			_shine.ExecuteModeUpdate();
	}
	public bool IsNormalLightOn() => _normalLightSource.activeSelf;
	public bool IsOnFire() => _onFire;
	public bool IsOnHand() => _isOnHand;
	public float GetCurrentNormalIntensity()
	{
		if(_lanternLights == null)
			_lanternLights = new Light[]
			{
				_normalLightSource.GetComponent<Light>(),
			    _directLightSource.GetComponent<Light>(),
			    _behindLightSource.GetComponent<Light>(),
				_onHandLight
			};

		return	_lanternLights[0].intensity;
	}
	public float GetCurrentDirectIntensity() => _lanternLights[1].intensity;
	public float GetCurrentBehindIntensity() => _lanternLights[2].intensity;
	public float GetCurrentWeaponIntensity() => _lanternLights[3].intensity;
	public float GetInitialNormalIntensity() => _initialIntensityNormal;
	public float GetInitialDirectIntensity() => _initialIntensityDirect;
	public float GetInitialBehindIntensity() => _initialIntensityBehind;
	public float GetInitialWeaponIntensity() => .5f;

	private void MoveNormal()
	{
		Y_Movement();

		float y = Mathf.InverseLerp(_yLimit.x, _yLimit.y, _pivot.transform.localPosition.y);
		_normalLightSource.transform.localPosition = (Vector3.up * y) + new Vector3(_normalLightSource.transform.localPosition.x, 3f, _normalLightSource.transform.localPosition.z);
		_directLightSource.transform.localPosition = (Vector3.up * y) + new Vector3(_directLightSource.transform.localPosition.x, 3f, _directLightSource.transform.localPosition.z);
	}
	private void MoveByCamera()
	{
		X_Movement();
		_normalLightSource.transform.localPosition = new Vector3((-_pivot.transform.localPosition.x - 1) * 1.5f, _normalLightSource.transform.localPosition.y, _normalLightSource.transform.localPosition.z);
		_directLightSource.transform.localPosition = new Vector3((-_pivot.transform.localPosition.x - 1) * 1.5f, _directLightSource.transform.localPosition.y, _directLightSource.transform.localPosition.z);
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
		if (ctx.performed && !_isFreeze && _onFire)
		{
			SetLightning(false);
			_isShining = _onFire;
			_lanternHelpers.LitMultiplier = 10;
		}
	}
	private void OnRightTriggerCanceled(InputAction.CallbackContext ctx)
	{
		if (ctx.canceled && !_isFreeze && _onFire)
		{
			SetLightning(true);
			_isShining = false;
			_lanternHelpers.LitMultiplier = 1;
			_shine.StopMode();
		}
	}
	private void OnEnable()
	{
		_animator?.SetBool("OnFire", _onFire);
	}
	private void OnDisable()
	{
		_isShining = false;

		if (_weaponHelpers.StopChange)
			Disable();
	}
	public void Disable()
	{
		_enlighting = false; 
		_shine.StopMode();
		_weaponHelpers.StopChange = false;
		_animator.ResetTrigger("Enlight");
		_animator.ResetTrigger("Delight");
		_gasLeakLoop.Stop();
		if (!_onFire)
		{
			_animator.Play("NotOnFire");
			_lanternHelpers.StopUsingGas();
		}
		else
		{
			_animator.Play("OnFire");
			_lanternHelpers.StartUsingGas();
			_gasLeakLoop.clip = _gasSound;
			_gasLeakLoop.Play();
		}
		transform.localPosition = _latestPos;
	}
	public void OnGasOut()
	{
		Delight();
	}
	private void HandleActivation()
	{
		if (_enlighting) return;

		if (!_onFire)
		{
			var item = _inventoryManager.GetItem(_match.ItemName.GetLocalizedString(), 1);

			if (item == null || _lanternHelpers.IsGasEmpty())
				return;

			_enlighting = true;
			_weaponHelpers.StopChange = true;
			_source.PlayOneShot(_matchSound);
			_animator.SetTrigger("Enlight");
			_inventoryManager.RemoveItemQuantityFromInventory(item, 1);
		}
		else
		{
			_lanternHelpers.StopUsingGas();
			_enlighting = true;
			_weaponHelpers.StopChange = true;
			_animator.SetTrigger("Delight");
		}
	}
	public void Enlight()
	{
		_onFire = true;
		_onHandLight.intensity = 0.5f;
		_gasLeakLoop.clip = _gasSound;
		_gasLeakLoop.Play();
		_animator.SetBool("OnFire", _onFire);
		_lanternHelpers.StartUsingGas();
		_enlighting = false;
		_weaponHelpers.StopChange = false;
		SetLightning(true);
	}
	public void Delight()
	{
		_onHandLight.intensity = 0f;
		_onFire = false;
		_isShining = false;
		_gasLeakLoop.Stop();
		_animator.SetBool("OnFire", _onFire);
		_weaponHelpers.StopChange = false;
		_enlighting = false;
		_shine.StopMode();
		SetLightning(false);
	}

	public void SetLightning(bool isDefault)
	{
		if (_onFire)
		{
			_normalLightSource.SetActive(isDefault);
			_directLightSource.SetActive(!isDefault);
			_playerLight.SetActive(false);
		}
		else
		{
			_normalLightSource.SetActive(false);
			_directLightSource.SetActive(false);
			_behindLightSource.SetActive(false);
			_playerLight.SetActive(true);
		}
	}
	public void Intensity(int id, float intensity)
	{
		if (_lanternLights == null)
			_lanternLights = new Light[] { _normalLightSource.GetComponent<Light>(), _directLightSource.GetComponent<Light>(), _behindLightSource.GetComponent<Light>() };

		_lanternLights[id].intensity = intensity;
	}
	public void SetBehindLightning(bool enable)
	{
		_behindLightSource.SetActive(enable);
	}
	public void SetLightningTurnOn(bool on)
	{
		_normalLightSource.SetActive(on);
		_directLightSource.SetActive(false);
		_playerLight.SetActive(false);
	}
	public override void OnSelected(ControlSchema schema)
	{
		_source.PlayOneShot(_switchIn);
		CanChange = false;
		_isOnHand = true;
		_controls = schema;

		_weaponHelpers.CheckSelected(_animator, this, _onFire ? "SelectedON" : "SelectedOFF");
	}

	public override void OnChanged()
	{
		_source.PlayOneShot(_switchOut);
		CanChange = false;
		_weaponHelpers.CheckOnChange(_animator, _controls, this, _onFire ? "ChangeON" : "ChangeOFF");
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

	public override void SetWeaponControls(bool setEnable)
	{
		if (setEnable)
		{
			_controls.Player.PrimaryShoot.performed += OnLeftTrigger;
			_controls.Player.SecondaryShoot.performed += OnRightTriggerPerformed;
			_controls.Player.SecondaryShoot.canceled += OnRightTriggerCanceled;

			_weaponHelpers.SetWeaponChange(false);

			_animator.SetBool("OnFire", _onFire);
			_normalLightSource.SetActive(_onFire);
			_behindLightSource.SetActive(false);
		}
		else
		{
			_isOnHand = false;
			if (_controls != null)
			{
				_controls.Player.PrimaryShoot.performed -= OnLeftTrigger;
				_controls.Player.SecondaryShoot.performed -= OnRightTriggerPerformed;
				_controls.Player.SecondaryShoot.canceled -= OnRightTriggerCanceled;
			}

			_lanternHelpers.LitMultiplier = 1;
			_directLightSource.SetActive(false);
			_normalLightSource.SetActive(false);
			_behindLightSource.SetActive(_onFire);
			if (_onFire && !_gasLeakLoop.isPlaying)
			{
				_gasLeakLoop.clip = _gasSound;
				_gasLeakLoop.Play();
			}

		}
	}
}
