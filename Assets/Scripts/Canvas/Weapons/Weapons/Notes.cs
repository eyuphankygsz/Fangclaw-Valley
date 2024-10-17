using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Notes : Weapons
{
	private Dictionary<int, IWeaponModes> _modes = new Dictionary<int, IWeaponModes>();
	private IWeaponModes _defaultMode;

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


	public override void OnSelected(ControlSchema schema){}

	public override void OnChanged() { }

	public override void SetWeapon() { }

	public override GameData GetSave()
	{
		return null;
	}

	public override void LoadSave()
	{
		return;
	}
}
