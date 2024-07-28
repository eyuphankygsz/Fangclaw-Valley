using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InteractableData
{
	public string InteractableName;
	public Vector3 Position;
	public bool IsActive;
	public SaveType Type;
}

[System.Serializable]
public class PickupData : InteractableData
{
	public bool IsPickedUp;
}
[System.Serializable]
public class HingedData : InteractableData
{
	public bool IsOn;
	public bool IsLocked;
}
[System.Serializable]
public class CrateData : InteractableData
{
	public bool IsShattered;
	public bool IsEmpty;
	public bool HasItem;
}
