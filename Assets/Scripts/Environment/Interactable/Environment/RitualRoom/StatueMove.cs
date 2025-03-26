using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatueMove : MonoBehaviour
{
	public void SetLockFalse(Interactable_PlaceHolder holder)
	{
		holder.IsLocked = false;
	}
	public void SetLockTrue(Interactable_PlaceHolder holder)
	{
		holder.IsLocked = true;
	}
}
