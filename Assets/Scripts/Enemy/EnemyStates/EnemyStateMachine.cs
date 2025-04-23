using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class EnemyStateMachine : MonoBehaviour
{
	private IEnemyState _currentState;

	[Inject]
	private InputManager _inputManager;



	[SerializeField]
	private EnemyStateTransitionList _transitionList;

	[SerializeField]
	private EnemyStateDict _enemyStateDict;

	private Dictionary<string, IEnemyState> _states;

	[SerializeField]
	private List<AbstractCondition> _allConditions;

	[SerializeField]
	private GameObject _controllerObj;

	private IEnemyController _controller;

	private bool _init;

	[SerializeField]
	private bool _debugged;
	private void Awake()
	{
		_controller = GetComponent<IEnemyController>();
	}

	private void Start()
	{
		_states = _enemyStateDict.ToDict();
	}

	public void SetCurrentState(IEnemyState state)
	{
		EnterState(state);
	}
	public IEnemyState GetCurrentState() => _currentState;
	public string GetCurrentStateName() => _states.First(x => x.Value == _currentState).Key;
	public void SetCurrentState(string stateName)
	{
		if (!_init)
		{
			_init = true;
			Start();
		}
		_controller.DiscardTime = true;
		EnterState(GetState(stateName));
	}

	public void Update()
	{
		_currentState.UpdateState();
		CheckTransitions();
	}

	private void EnterState(IEnemyState state)
	{
		if (_debugged)
			Debug.Log("OLDSTATE: " + _currentState + " NEWSTATE: " + state);

		_transitionList = state.GetTransitions();

		_currentState?.ExitState();
		_currentState = state;
		_currentState.EnterState();

	}



	private void CheckTransitions()
	{

		if (_transitionList == null)
			return;


		for (int i = 0; i < _transitionList.Transitions.Count; i++)
		{
			var transition = _transitionList.Transitions[i];

			bool canChange = true;
			foreach (var item in transition.Conditions)
			{
				if (item.Condition.CheckCondition() != item.IsTrue)
				{
					canChange = false;
				}
			}

			if (canChange)
				SetCurrentState(GetState(transition.TransitionName));
		}

		foreach (var item in _allConditions)
		{
			item.ResetFrameFreeze();
		}

	}
	private IEnemyState GetState(string name)
	{
		return _states[name];
	}
}


[System.Serializable]
public class EnemyStateDict
{
	[SerializeField]
	List<PlayerState> _states;

	public Dictionary<string, IEnemyState> ToDict()
	{
		Dictionary<string, IEnemyState> states = new Dictionary<string, IEnemyState>();
		foreach (var item in _states)
			states.Add(item.Name, item.TheState.GetComponent<IEnemyState>());

		return states;
	}
}

[System.Serializable]
public class EnemyState
{
	public string Name;
	public MonoBehaviour TheState;
}
