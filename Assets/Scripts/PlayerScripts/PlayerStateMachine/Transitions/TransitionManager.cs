using System.Collections.Generic;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
	[SerializeField]
	private StateTransitionList _transitionList;
	private PlayerStateMachine _playerStateMachine;

	[SerializeField]
	private PlayerStateDict _playerStateDict;
	[SerializeField]
	private NoStateLock _stateLock;

	private Dictionary<string, IPlayerState> _states;


	private void Awake()
	{
		_playerStateMachine = GetComponent<PlayerStateMachine>();
		_states = _playerStateDict.ToDict();
	}
	public void SetTransitionList(StateTransitionList transitionList)
	{
		_transitionList = transitionList;
	}

	public void CheckTransitions(ControlSchema controls)
	{

		if(_transitionList == null)
			return;

		foreach (var transition in _transitionList.Transitions)
		{
			bool canChange = true;

			foreach (var item in transition.Conditions)
				if (!item.CheckCondition())
				{
					canChange = false;
					break;
				}

			if (canChange)
				if (!_stateLock.Lock)
					_playerStateMachine.SetCurrentState(GetState(transition.TransitionName));
		}
	}
	private IPlayerState GetState(string name)
	{
		return _states[name];
	}
}


[System.Serializable]
public class PlayerStateDict
{
	[SerializeField]
	List<PlayerState> _states;

	public Dictionary<string, IPlayerState> ToDict()
	{
		Dictionary<string, IPlayerState> states = new Dictionary<string, IPlayerState>();
		foreach (var item in _states)
			states.Add(item.Name, item.TheState.GetComponent<IPlayerState>());

		return states;
	}
}

[System.Serializable]
public class PlayerState
{
	public string Name;
	public MonoBehaviour TheState;
}

[System.Serializable]
public class StateTransitionList
{
	public List<StateTransition> Transitions;
}
[System.Serializable]
public class StateTransition
{
	public string TransitionName;
	public List<AbstractCondition> Conditions;
}

