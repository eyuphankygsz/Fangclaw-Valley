using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
	private IPlayerState _currentState;
	private IPlayerState _startState;

	private void Start()
	{
		SetCurrentState(_startState);
	}
	
	public void SetCurrentState(IPlayerState state) 
		=> EnterState(state);
	
	public void ExecuteState() 
		=> _currentState.UpdateState();
	

	private void EnterState(IPlayerState state)
	{
		if (_currentState != null)
			_currentState.ExitState();

		_currentState = state;
		_currentState.EnterState();
	}


}
