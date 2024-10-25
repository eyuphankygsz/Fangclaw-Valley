using UnityEngine;
using Zenject;

public class EnemyStateMachine : MonoBehaviour
{
	private IEnemyState _currentState;
	[SerializeField]
	private MonoBehaviour _startState;

	private TransitionManager _transitionManager;

	[Inject]
	private InputManager _inputManager;

	private void Start()
	{
		_transitionManager = GetComponent<TransitionManager>();
		SetCurrentState(_startState.GetComponent<IEnemyState>());
	}

	public void SetCurrentState(IEnemyState state)
	{
		EnterState(state);
	}

	public void ExecuteState()
	{
		_currentState.UpdateState();
		_transitionManager.CheckTransitions(_inputManager.Controls);
	}

	private void EnterState(IEnemyState state)
	{
		_currentState?.ExitState();
		_currentState = state;
		_currentState.EnterState();

		_transitionManager.SetTransitionList(_currentState.GetTransitions());
	}
}
