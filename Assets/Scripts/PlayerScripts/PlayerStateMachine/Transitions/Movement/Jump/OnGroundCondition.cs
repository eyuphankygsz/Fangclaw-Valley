using UnityEngine;

public class OnGroundCondition : AbstractCondition
{
	[SerializeField]
	private PlayerGroundCheck _groundCheck;

	public override bool CheckCondition()
	{
		return _groundCheck.IsOnGround();
	}

    public override void ResetFrameFreeze()
    {
		
    }
}
