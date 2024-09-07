using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Lantern : Weapons
{
	[SerializeField] private GameObject _normalLightSource;
	[SerializeField] private GameObject _directLightSource;

	private bool _active;

	private float _leftFuel;
	private bool _onFire;

	private Dictionary<int, IWeaponModes> _modes = new Dictionary<int, IWeaponModes>();
	private IWeaponModes _defaultMode;

	private LanternData _data = new LanternData();
	public override void Move()
	{
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
			_active = !_active;
			_onFire = _active;
			_normalLightSource.SetActive(_active);
			SetLightning(true);
		}
		if (Input.GetMouseButtonDown(1))
			SetLightning(false);
		else if (Input.GetMouseButtonUp(1))
			SetLightning(true);
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
	}

	public override void OnChanged()
	{
		_active = false;
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
			Debug.Log(gameObject.name);
			gameObject.SetActive(false);
			return;
		}

		IsPicked = data.IsPicked;
		_leftFuel = data.LeftFuel;
		_data = data;
		
		gameObject.SetActive(data.IsSelected);
	}
}
