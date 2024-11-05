public class IsPlayerHiding : AbstractCondition
{
    private PlayerController _controller;

	public override bool CheckCondition()
	{
        return _controller.Hiding;
	}
}
