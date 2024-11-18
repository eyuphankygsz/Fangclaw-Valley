using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class IsPlayerClose : AbstractCondition
{
	[SerializeField]
	private Transform _target;
	[SerializeField]
	private float _range;

	public override bool CheckCondition()
	{
		return Vector3.Distance(transform.position, _target.position) < _range;
	}
	
    public override void ResetFrameFreeze() { }
}
