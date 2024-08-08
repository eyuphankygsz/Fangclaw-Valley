using System.Collections.Generic;
using UnityEngine.InputSystem;

public interface IPlayerState
{
	void EnterState();
	void UpdateState();
	void ExitState();
	void OnInputEnable(ControlSchema schema);
	StateTransitionList GetTransitions();
}