using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CrateItemsData
{
	public List<CrateItem> Items;
}

[Serializable]
public class CrateItem
{
	public string ItemName;
	public Vector3 Position;
	public bool Taken;
}