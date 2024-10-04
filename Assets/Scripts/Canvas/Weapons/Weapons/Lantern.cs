using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Lantern : Weapons
{
	[SerializeField] private GameObject _normalLightSource;
	[SerializeField] private GameObject _directLightSource;
	[SerializeField] private InventoryItem _match;
	[SerializeField] private AudioClip _matchSound;

	private bool _active;

	private float _leftFuel;
	private bool _onFire;

	private Dictionary<int, IWeaponModes> _modes = new Dictionary<int, IWeaponModes>();
	private IWeaponModes _defaultMode;

	private LanternData _data = new LanternData();

	[Inject]
	private InventoryManager _inventoryManager;
	private bool _enlighting;
	public override void Move()
	{
		if(_enlighting) return;
		MoveNormal();
		MoveByCamera();
		ClampMove();
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

	public override void OnAction()
	{
		if (Input.GetMouseButtonDown(0))
		{
			HandleActivation();
		}
		if (Input.GetMouseButtonDown(1))
			SetLightning(false);
		else if (Input.GetMouseButtonUp(1))
			SetLightning(true);
	}

	private void HandleActivation()
	{
		if (_enlighting) return;

		if (!_active)
		{
			var item = _inventoryManager.GetItem(_match.ItemName.GetLocalizedString(), 1);
			if (item == null) return;
			_enlighting = true;
			_source.clip = _matchSound;
			_source.Play();
			_animator.SetTrigger("Enlight");
			_inventoryManager.RemoveItemQuantityFromInventory(item, 1);
		}
		else
		{
			_enlighting = true;
			_animator.SetTrigger("Delight");
		}
	}
	public void Enlight()
	{
		_active = true;
		_onFire = true;
		_enlighting = false;
		SetLightning(true);
	}
	public void Delight()
	{
		_active = false;
		_onFire = false;
		_enlighting = false;
		SetLightning(false);
	}
	private void SetLightning(bool isDefault)
	{
		if (_active)
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

	public override void OnSelected()
	{
		_normalLightSource.SetActive(_onFire);
	}

	public override void OnChanged()
	{
		_normalLightSource.SetActive(false);
		_directLightSource.SetActive(false);
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
			LeftFuel = _leftFuel,
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
		_leftFuel = data.LeftFuel;
		_data = data;

		gameObject.SetActive(data.IsSelected);
	}
}
