using UnityEngine;

[System.Serializable]
public class WeaponData : GameData
{
	public bool IsSelected;
	public bool IsPicked;
}

[System.Serializable]
public class LanternData : WeaponData
{
	public float LeftFuel;
	public bool OnFire;
}

[System.Serializable]
public class WeaponPickData : WeaponData
{
	public bool IsTaken;
}