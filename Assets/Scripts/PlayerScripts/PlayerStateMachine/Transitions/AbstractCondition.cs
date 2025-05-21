using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractCondition : MonoBehaviour
{
	public abstract bool CheckCondition();
	public abstract void ResetFrameFreeze();
	public bool DebugCondition;
}


[System.Serializable]
public class EnemyStateTransitionList
{
	public List<EnemyStateTransition> Transitions;
}
[System.Serializable]
public class EnemyStateTransition
{
	public string TransitionName;
	public List<EnemyStateTransitionInterior> Conditions;
}
[System.Serializable]
public class EnemyStateTransitionInterior
{
	public AbstractCondition Condition;
	public bool IsTrue;
}