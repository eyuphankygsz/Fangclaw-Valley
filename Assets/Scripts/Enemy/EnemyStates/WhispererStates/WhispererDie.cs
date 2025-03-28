using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhispererDie : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private Animator _animator;
	[SerializeField]
	private AudioSource _src;
	[SerializeField]
	private AudioClip _clip;

	public void EnterState()
	{
		_animator.SetBool("Die", true);

	}

	public void ExitState()
	{
		_animator.SetBool("Die", false);
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return null;
	}

	public void UpdateState()
	{

	}
}
