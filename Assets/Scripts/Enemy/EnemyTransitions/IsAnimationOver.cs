public class IsAnimationOver : AbstractCondition
{
	private bool _over;

	public void SetOver(bool over) => _over = over;
	public override bool CheckCondition()
	{
		return _over;	
	}
	public override void ResetFrameFreeze() { }
}
