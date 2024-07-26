using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_HingedObjects : Interactable
{
	[SerializeField]
	private LockKey _lockKey;

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
		if (IsLocked() || _animating) return;

		_isOn = !_isOn;
		_animator.SetBool("On", _isOn);
		_animating = true;
	}

	public void AnimationOver()
	{
		_animating = false;
	}
	private bool IsLocked()
	{
		if (_lockKey.Locked)
			if (PlayerPrefs.GetString(_lockKey.KeyName) != "Taken") return true;

		return false;
	}
}
