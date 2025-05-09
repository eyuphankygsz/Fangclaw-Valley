using System.Collections.Generic;
using UnityEngine;

public class GPiggyHit : MonoBehaviour, IEnemyState
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
	private IsDied _health;
	[SerializeField]
	private TimeForExitHit _timeForExit;

	[SerializeField]
	private IsAnimationOver _isAnimOver;
	private void Awake()
	{
	}


	public void EnterState()
	{
		_health.UpdateHealth(-1);
		_isAnimOver.SetOver(true);
	}

	public void ExitState()
	{
		_isAnimOver.SetOver(false);
	}

	public EnemyStateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{

	}
}
