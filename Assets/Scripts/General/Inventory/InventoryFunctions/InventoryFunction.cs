using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InspectFunctions : MonoBehaviour
{
	public abstract void Inspect(InventoryItemHolder lastSelectedHolder);
}
public abstract class UseFunctions : MonoBehaviour
{
	public abstract void Use(InventoryItemHolder lastSelectedHolder);
}
public abstract class CombineFunctions : MonoBehaviour
{
	public abstract void Combine(InventoryItemHolder lastSelectedHolder);
}
public abstract class DropFunctions : MonoBehaviour
{
	public abstract void Drop(InventoryItemHolder lastSelectedHolder);
}
