using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class IsInAttackRange : AbstractCondition
{
	[SerializeField]
	private Transform _target;
	[SerializeField]
	private float _range;

	[SerializeField]
	private bool _showRange;
	public override bool CheckCondition()
	{
		return Vector3.Distance(transform.position, _target.position) < _range;
	}
	private void OnDrawGizmos()
	{
		if (!_showRange) return;
		Vector3 targetPos = _target.position;
		Vector3 pos = transform.position;

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(pos, targetPos);

		Vector3 mid = (targetPos + pos) / 2;

		GUIStyle style = new GUIStyle();
		style.normal.textColor = Color.white;
		Handles.Label(mid, Vector3.Distance(transform.position, _target.position).ToString("F2") + " units", style);

	}
}
