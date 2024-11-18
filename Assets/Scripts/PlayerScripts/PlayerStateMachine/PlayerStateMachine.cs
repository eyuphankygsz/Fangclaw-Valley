using UnityEngine;
using Zenject;

public class PlayerStateMachine : MonoBehaviour
{
	private IPlayerState _currentState;
	[SerializeField]
	private MonoBehaviour _startState;

	private TransitionManager _transitionManager;
	
	[Inject]
	private InputManager _inputManager;

	private void Start()
	{
		_transitionManager = GetComponent<TransitionManager>();
		SetCurrentState(_startState.GetComponent<IPlayerState>());
	}

	public void SetCurrentState(IPlayerState state)
	{
		EnterState(state);
	}

	public void ExecuteState()
	{
		_currentState.UpdateState();
		_transitionManager.CheckTransitions(_inputManager.Controls);
	}

	private void EnterState(IPlayerState state)
	{
		Debug.Log(state);
		_currentState?.ExitState();
		_currentState = state;
		_currentState.EnterState();
		_currentState.OnInputEnable(_inputManager.Controls);

		_transitionManager.SetTransitionList(_currentState.GetTransitions());
	}
}
