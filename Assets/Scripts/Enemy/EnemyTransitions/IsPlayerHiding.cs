using UnityEngine;

public class IsPlayerHiding : AbstractCondition
{
    [SerializeField]
    private PlayerController _controller;

    private bool _isHiding;
	public override bool CheckCondition()
	{
        if(DebugCondition)
            Debug.Log("IsPlayerHiding: " + _isHiding);
            
        _isHiding = _controller.Hiding;
        return _controller.Hiding;
	}

    public override void ResetFrameFreeze()
    {

    }
}
