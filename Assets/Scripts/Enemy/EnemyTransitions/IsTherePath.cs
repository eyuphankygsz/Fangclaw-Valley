using UnityEngine;
using UnityEngine.AI;

public class IsTherePath : AbstractCondition
{
	[SerializeField]
	private NavMeshAgent _agent;
	
	public override bool CheckCondition()
	{
		return _agent.pathPending || _agent.hasPath;
	}

	public override void ResetFrameFreeze() { }
}
