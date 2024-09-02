using System;
using System.Collections;
using UnityEngine;
public class WeaponHelpers : MonoBehaviour
{
	public void CooldownGun(float time, Action action)
	{
		StartCoroutine(StartCooldown(time, action));
	}
	private IEnumerator StartCooldown(float time, Action action)
	{
		yield return new WaitForSeconds(time);
		Debug.Log("ACTION");
		action.Invoke();
	}
}
