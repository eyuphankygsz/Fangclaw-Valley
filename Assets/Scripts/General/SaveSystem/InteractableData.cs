using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PickupData : GameData
{
	public bool IsPickedUp;
}
[System.Serializable]
public class HingedData : GameData
{
	public bool Used;
	public bool IsOn;
	public bool IsLocked;
}
[System.Serializable]
public class CrateData : GameData
{
	public bool IsShattered;
}
[System.Serializable]
public class SwitchData : GameData
{
	public bool IsOn;
}

[System.Serializable]
public class PlaceHolderData : GameData
{
	public bool IsActive;
}
