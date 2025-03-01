using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhispererStun : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private Animator _animator;
	[SerializeField]
	private EnemyStateTransitionList _transitions;
	[SerializeField]
	private WhispererController _controller;
	[SerializeField]
	private AudioSource _src;
	[SerializeField]
	private AudioClip[] _clips;
	private int[] _oldClips = new int[2] { -1, -1 };
	private int _oldIndex;
	
	List<int> clipIndexs = new List<int>();
	public void EnterState()
	{
		clipIndexs.Clear();
        for (int i = 0; i < _clips.Length; i++)
        {
			bool found = false;
			
			for (int j = 0; j < _oldClips.Length; j++)
				if (_oldClips[j] == i)
					found = true;

			if(!found)
				clipIndexs.Add(i);
        }

		int rand = Random.Range(0, clipIndexs.Count);
		_src.PlayOneShot(_clips[rand]);
		_oldClips[_oldIndex++] = rand;
		_oldIndex %= _oldClips.Length;

        _animator.SetBool("Stun", true);
	}

	public void ExitState()
	{
		_controller.Stunned = false;
		_animator.SetBool("Stun", false);
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{

	}
}
