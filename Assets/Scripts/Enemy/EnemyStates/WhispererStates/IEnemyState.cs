public interface IEnemyState
{
	void EnterState();
	void UpdateState();
	void ExitState();
	EnemyStateTransitionList GetTransitions();
}
