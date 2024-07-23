using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Chests : Interactable
{
	[SerializeField]
	private bool _locked;
	[SerializeField]
	private bool _isOn;
	private bool _animating;
	private Animator _animator;

	private void Awake()
	{
		_animator = GetComponent<Animator>();
	}
	public override void OnInteract(Enum_Weapons weapon)
	{
		if (_locked || _animating) return; //print screen message for 2 seconds. (or play audio)

		_isOn = !_isOn;
		_animator.SetBool("On", _isOn);
		_animating = true;
	}

	public void AnimationOver()
	{
		_animating = false;
	}
}
