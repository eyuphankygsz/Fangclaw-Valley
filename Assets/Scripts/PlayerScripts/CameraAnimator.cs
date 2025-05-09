using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

// Handles camera animation playback and interaction with PlayerForce
public class CameraAnimator : MonoBehaviour
{
	private Animator _animator;
	private string _animationName;
	[SerializeField]
	private PlayerForce _playerForce;

	// Called on script start, initializes the Animator reference
	void Start()
	{
		_animator = GetComponent<Animator>();
	}

	// Plays the specified animation and starts coroutine to disable animator after playback
	public void PlayAnimation(string animationName)
	{
		_animator.enabled = true;
		_animationName = animationName;
		_animator.Play(animationName);
		StartCoroutine(DisableAnimator());
	}

	// Stops the player's force by calling StopForce on PlayerForce
	public void StopForce() =>
		_playerForce.StopForce();

	// Coroutine that waits until the animation finishes before disabling the animator
	private IEnumerator DisableAnimator()
	{
		yield return new WaitForSeconds(0.1f);
		while (_animator.GetCurrentAnimatorStateInfo(0).IsName(_animationName))
		{
			yield return null;
		}
	}
}
