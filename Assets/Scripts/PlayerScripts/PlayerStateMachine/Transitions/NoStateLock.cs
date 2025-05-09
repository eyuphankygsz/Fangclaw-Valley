public class NoStateLock : AbstractCondition
{
	public bool Lock;

	public override bool CheckCondition() => Lock;
    public void SetLock(bool theLock) => Lock = theLock;

    public override void ResetFrameFreeze()
    {

    }
}
