using System;
using System.Collections;
using UnityEngine;
public class WeaponHelpers : MonoBehaviour
{
	public bool StopChange;
	public void CooldownGun(float time, Action action)
	{
		StartCoroutine(StartCooldown(time, action));
	}
	private IEnumerator StartCooldown(float time, Action action)
	{
		yield return new WaitForSeconds(time);
		action.Invoke();
	}



	#region Transition

	private PlayerWeaponController _weaponController;
	public void SetWeaponController(PlayerWeaponController controller)
	{
		_weaponController = controller;
	}
	public void CheckSelected(Animator animator, Weapons weapon, string animation)
	{
		StartCoroutine(CheckSelectedAnim(animator, weapon, animation));
	}
	public void CheckOnChange(Animator animator, ControlSchema controls, Weapons weapon, string animation)
	{
		StartCoroutine(CheckChangeAnim(animator, controls, weapon, animation));
	}
	private IEnumerator CheckSelectedAnim(Animator animator, Weapons weapon, string animation)
	{
		weapon.gameObject.SetActive(true);
		animator.Play(animation, 0);
		yield return new WaitForEndOfFrame();
		while (true)
		{
			if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animation))
			{
				weapon.SetWeaponControls(true);
				weapon.CanChange = true;
				yield break;
			}
			yield return null;
		}
	}
	private IEnumerator CheckChangeAnim(Animator animator, ControlSchema controls, Weapons weapon, string animation)
	{
		animator.Play(animation, 0);
		yield return new WaitForEndOfFrame();
		while (true)
		{
			if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animation))
			{
				if (controls != null)
					weapon.SetWeaponControls(false);
				weapon.CanChange = true;
				weapon.gameObject.SetActive(false);
				yield break;
			}
			yield return null;
		}
	}

	public void SetWeaponChange(bool canChange)
	{
		_weaponController.OnWeaponChanging = canChange;
	}
	#endregion
}
