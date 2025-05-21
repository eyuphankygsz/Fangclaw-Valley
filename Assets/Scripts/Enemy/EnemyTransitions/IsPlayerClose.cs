using UnityEngine;

public class IsPlayerClose : AbstractCondition
{
	[SerializeField]
	private Transform _target;
	[SerializeField]
	private float _range;

	public override bool CheckCondition()
	{
		bool isClose = Mathf.Abs(Vector3.Distance(transform.position, _target.position)) < _range;
		if(DebugCondition)
		{
			Debug.Log("IsPlayerClose: " + isClose);
			if(isClose)
				Debug.Log("Distance: " + Vector3.Distance(transform.position, _target.position));
		}

		return isClose;
	}
	
    public override void ResetFrameFreeze() { }

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, _range);
	}
}
