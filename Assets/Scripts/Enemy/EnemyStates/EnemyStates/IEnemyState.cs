using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyState
{
	void EnterState();
	void UpdateState();
	void ExitState();
	StateTransitionList GetTransitions();
}
