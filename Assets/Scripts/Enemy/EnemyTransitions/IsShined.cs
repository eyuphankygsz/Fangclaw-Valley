using UnityEngine;

public class IsShined : AbstractCondition
{

	[SerializeField]
	private GameObject _parent;
	private IEnemyController _target;

	private void Awake()
	{
		_target = _parent.GetComponent<IEnemyController>();
	}
	public override bool CheckCondition()
	{
		return _target.IsShined();
	}
	
    public override void ResetFrameFreeze() { }
}
