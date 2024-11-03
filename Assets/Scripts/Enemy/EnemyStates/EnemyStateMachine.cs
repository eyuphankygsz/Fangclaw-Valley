using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class EnemyStateMachine : MonoBehaviour
{
	private IEnemyState _currentState;
	[SerializeField]
	private MonoBehaviour _startState;

	[Inject]
	private InputManager _inputManager;



    [SerializeField]
    private EnemyStateTransitionList _transitionList;

    [SerializeField]
    private EnemyStateDict _enemyStateDict;

    private Dictionary<string, IEnemyState> _states;


    private void Start()
	{
        _states = _enemyStateDict.ToDict();

        SetCurrentState(_startState.GetComponent<IEnemyState>());
	}

    public void SetCurrentState(IEnemyState state)
    {
        EnterState(state);
    }
    public void SetCurrentState(string stateName)
    {
        EnterState(GetState(stateName));
    }

    public void Update()
	{
		_currentState.UpdateState();
		CheckTransitions();
	}

	private void EnterState(IEnemyState state)
	{
		_currentState?.ExitState();
        _currentState = state;
        _transitionList = _currentState.GetTransitions();


		_currentState.EnterState();

	}



	private void CheckTransitions()
	{
        foreach (var transition in _transitionList.Transitions)
        {
            bool canChange = true;

            foreach (var item in transition.Conditions)
                if (item.Condition.CheckCondition() != item.IsTrue)
                {
                    canChange = false;
                    break;
                }

            if (canChange)
                SetCurrentState(GetState(transition.TransitionName));
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
