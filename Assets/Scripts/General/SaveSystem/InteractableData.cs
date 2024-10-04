using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PickupData : GameData
{
	public bool IsPickedUp;
	public int LeftQuantity;
	public Vector3 Position;
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
public class RotateDiscData : GameData
{
	public int SelectedID;
}

[System.Serializable]
public class PlaceHolderData : GameData
{
	public bool IsItemOn;
	public int StatusID;
}

[System.Serializable]
public class CombLockData : GameData
{
	public List<int> LastIDList;
	public bool IsUnlocked;
}
[System.Serializable]
public class MusicMechData : GameData
{
	public string CurrentOrder;
	public bool IsDone;
}
