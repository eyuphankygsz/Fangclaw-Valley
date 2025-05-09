using System.Collections.Generic;
using UnityEngine;

public class WhispererHit : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private Animator _animator;
	[SerializeField]
	private EnemyStateTransitionList _transitions;
	[SerializeField]
	private AudioSource _src;
	[SerializeField]
	private AudioClip[] _clips;
	private int[] _oldClips = new int[2] { -1, -1 };
	private int _oldIndex;

	List<int> clipIndexs = new List<int>();

	[SerializeField]
	private IEnemyController _controller;

	[SerializeField]
	private IsDied _health;
	[SerializeField]
	private TimeForExitHit _timeForExit;

	[SerializeField]
	private IsAnimationOver _isAnimOver;
	private void Awake()
	{
		_controller = GetComponentInParent<IEnemyController>();
	}


	public void EnterState()
	{

		_isAnimOver.SetOver(false);
		_timeForExit.ResetTime();

		if (_clips.Length > 0)
		{
			clipIndexs.Clear();
			for (int i = 0; i < _clips.Length; i++)
			{
				bool found = false;

				for (int j = 0; j < _oldClips.Length; j++)
					if (_oldClips[j] == i)
						found = true;

				if (!found)
					clipIndexs.Add(i);
			}

			int rand = Random.Range(0, clipIndexs.Count);
			_src.PlayOneShot(_clips[rand]);
			_oldClips[_oldIndex++] = rand;
			_oldIndex %= _oldClips.Length;
		}

		_animator.SetBool("Hit", true);
		_controller.StartAnimationCheck("Hit");
		_health.UpdateHealth(-1);
	}

	public void ExitState()
	{
		_controller.Stunned = false;
		_animator.SetBool("Hit", false);
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{

	}
}
