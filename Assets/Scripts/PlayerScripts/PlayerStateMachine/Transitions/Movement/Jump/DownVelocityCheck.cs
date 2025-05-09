using UnityEngine;

public class DownVelocityCheck : AbstractCondition
{
    [SerializeField]
    private CharacterController _controller;

    public override bool CheckCondition()
    {
        if (_controller.velocity.y <= 0)
            return true;
        return false;
    }

    public override void ResetFrameFreeze()
    {

    }
}
