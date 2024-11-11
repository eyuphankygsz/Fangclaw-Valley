using UnityEngine;

public class IsPlayerHiding : AbstractCondition
{
    [SerializeField]
    private PlayerController _controller;

	public override bool CheckCondition()
	{
        return _controller.Hiding;
	}

    public override void ResetFrameFreeze()
    {

    }
}
