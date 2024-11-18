using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

public class TakeHideTable : MonoBehaviour
{
	[SerializeField]
	private Transform _playerTf;

	[SerializeField]
	private Transform _enemyTf;

	[SerializeField]
	private PlayableDirector _playable;


	public Transform GetPlayerTransform() => _playerTf;
	public Transform GetEnemyTransform() => _enemyTf;
	public PlayableDirector GetPlayable() => _playable;
}
