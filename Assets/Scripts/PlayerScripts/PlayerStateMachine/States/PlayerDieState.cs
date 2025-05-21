using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerDieState : MonoBehaviour, IPlayerState, IInputHandler
{
	[SerializeField]
	private CharacterController _controller;

	private ControlSchema _controls;
	[SerializeField]
	private GameObject _dieScreen;
	[SerializeField]
	StateTransitionList _transitions;
	[Inject]
	private GameManager _manager;

	public void EnterState()
	{
		_dieScreen.SetActive(true);
		_manager.SetPauseGame(true);
		_manager.IsDied = true;
	}

	public void UpdateState()
	{
	}

	public void ExitState()
	{
		OnInputDisable();
	}

	public void OnInputEnable(ControlSchema schema)
	{
		_controls = schema; 
	}

	public void OnInputDisable()
	{
	}

	public StateTransitionList GetTransitions()
	{
		return null;
	}



	//private void HandleUI()
	//{
	//	_playerUI.ChangeStaminaBar(_stamina);
	//}

	//public List<ICondition> GetTransitions()
	//{
	//	return _transitionList;
	//}
}
