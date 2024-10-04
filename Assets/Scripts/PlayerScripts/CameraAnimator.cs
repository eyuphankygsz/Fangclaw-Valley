using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
	private Animator _animator;
	private string _animationName;
	[SerializeField]
	private PlayerForce _playerForce;
	void Start()
	{
		_animator = GetComponent<Animator>();
	}

	public void PlayAnimation(string animationName)
	{
		_animator.enabled = true;
		_animationName = animationName;
		_animator.Play(animationName);
		StartCoroutine(DisableAnimator());
	}
	public void StopForce() =>
		_playerForce.StopForce();

	private IEnumerator DisableAnimator()
	{
		yield return new WaitForSeconds(0.1f);
		while (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationName))
		{
			yield return null;
		}
		_animator.enabled = false;
	}
}
