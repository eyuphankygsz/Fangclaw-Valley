using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Playables;

public class EnemyHideAttack : MonoBehaviour, IEnemyState
{
	[SerializeField]
	private NavMeshAgent _agent;
	[SerializeField]
	private NavMeshAgent _animator;
	[SerializeField]
	private float _speed, _radius;

	[SerializeField]
	EnemyStateTransitionList _transitions;
	[SerializeField]
	private LayerMask _layer;

	[SerializeField]
	private UnityEvent _events;
	[SerializeField]
	private PlayerForce _force;
	[SerializeField]
	private NoStateLock _playerStateLock;
	[SerializeField]
	private Transform _enemy, _player;

	[SerializeField]
	private IsAnimationOver _isAnimOver;
	[SerializeField]
	private HideOut _exitForHide;
	[SerializeField]
	private Animator _anim;

	private bool _reached, _started;
	private Transform _destination;
	private Transform _forceTransform;
	private PlayableDirector _director;


	private Vector3 _pos;

	[SerializeField]
	private SteamAchievements _achievements;
	[SerializeField]
	private AchievementCheck _noSafePlace;


	public void EnterState()
	{
		_achievements.TryEnableAchievement(_noSafePlace);


		_exitForHide.SetHideAttack();
		_isAnimOver.SetOver(false);
		_agent.stoppingDistance = 0;
		_pos = _player.transform.position;


		Collider[] colls = Physics.OverlapSphere(_pos, 1, _layer);
		if (colls.Length > 0)
		{
			TakeHideTable table = colls[0].transform.GetComponent<TakeHideTable>();
			_destination = table.GetEnemyTransform();
			_forceTransform = table.GetPlayerTransform();
			_director = table.GetPlayable();

			_agent.SetDestination(table.GetEnemyTransform().position);


			OnLookEvents onLook = new GameObject().AddComponent<OnLookEvents>();
			onLook.gameObject.SetActive(false);
			onLook.ForceEvents = _events;

			_force.SetEvents(onLook);
		}

	}

	public void ExitState()
	{
		_anim.SetBool("Search", false);
		_isAnimOver.SetOver(true);
		_reached = false;
		_started = false;
	}
	public EnemyStateTransitionList GetTransitions()
	{
		return _transitions;
	}

	public void UpdateState()
	{

		if ( !_reached && _agent.remainingDistance <= _agent.stoppingDistance + 0.1f)
		{
			_reached = true;
			Debug.Log("REACHED: " + _reached);
		}
		if (_reached && !_started)
		{
			Debug.Log("REACHED: " + _reached);
			_anim.SetBool("Search", true);
			_started = true;
			_enemy.transform.rotation = _destination.rotation;
			_enemy.transform.position = _destination.position;
			_force.StartForce(_forceTransform);
		}
	}
	public void PlayDirector()
	{
		_director.Play();
	}
	public void UnFreezePlayer()
	{
		_playerStateLock.Lock = false;
	}

	private void OnDrawGizmos()
	{
		if (_pos == null)
			return;

		Gizmos.DrawLine(transform.position, _pos);
		Gizmos.DrawWireSphere(_pos, 1);
	}
}
