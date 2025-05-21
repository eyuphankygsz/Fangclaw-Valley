using UnityEngine;

public class IsAnimationOver : AbstractCondition
{
	private bool _over;

	public void SetOver(bool over) => _over = over;
	public override bool CheckCondition()
	{
		if(DebugCondition)
			Debug.Log("IsAnimationOver: " + _over);
		return _over;	
	}
	public override void ResetFrameFreeze() { }
}
