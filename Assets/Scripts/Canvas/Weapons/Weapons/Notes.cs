using System.Collections.Generic;
using UnityEngine;

public class Notes : Weapons
{
	private Dictionary<int, IWeaponModes> _modes = new Dictionary<int, IWeaponModes>();
	private IWeaponModes _defaultMode;

	[SerializeField] private AudioClip _switchIn, _switchOut;


	private WeaponData _data = new WeaponData();

	public override void Move()
	{
		MoveNormal();
		MoveByCamera();
		ClampMove();
	}
	private void MoveNormal()
	{
		Y_Movement();
	}
	private void MoveByCamera()
	{
		X_Movement();
	}
	private void ClampMove()
	{
		ClampTransform();
	}


	public override void OnSelected(ControlSchema schema)
	{
		_source.PlayOneShot(_switchIn);
		CanChange = false;
		_weaponHelpers.CheckSelected(_animator, this, "Selected");
	}

	public override void OnChanged() 
	{
		_source.PlayOneShot(_switchOut);
		CanChange = false;
		_weaponHelpers.CheckOnChange(_animator, _controls, this, "OnChanged");	
	}

	public override void SetWeapon() { }

	public override GameData GetSave()
	{
		return null;
	}

	public override void LoadSave()
	{
		return;
	}

	public override void SetWeaponControls(bool setEnable)
	{
		if (setEnable)
		{
			_weaponHelpers.SetWeaponChange(false);

		}
		else
		{
			gameObject.SetActive(false);

		}
	}
}
