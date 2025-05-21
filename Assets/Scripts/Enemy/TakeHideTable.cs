using UnityEngine;
using UnityEngine.Playables;
using Zenject;

public class TakeHideTable : MonoBehaviour
{
	[SerializeField]
	private Transform _playerTf;

	[SerializeField]
	private Transform _enemyTf;

	[SerializeField]
	private PlayableDirector _playable;

	[SerializeField]
	private TalkEvents[] _talkEvents;

	public Transform GetPlayerTransform() => _playerTf;
	public Transform GetEnemyTransform() => _enemyTf;
	public PlayableDirector GetPlayable() => _playable;

	[Inject]
	private SteamAchievements _steamAch;
	[SerializeField]
	private AchievementCheck _creatureStun;
	public void TalkEventPlay()
	{
		if (_talkEvents.Length > 0)
			_talkEvents[Random.Range(0, _talkEvents.Length)].SelectTalkList();

		PlayerPrefs.SetInt("no_safe_place", PlayerPrefs.GetInt("no_safe_place", 0) + 1);
		if(_steamAch != null)
			_steamAch.TryEnableAchievement(_creatureStun);
	}
}
