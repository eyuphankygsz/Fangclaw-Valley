
public class DidPigSpawned : AbstractCondition
{

	public bool PigSpawned;
	private bool _tempPigSpawned;
	public override bool CheckCondition()
	{
		_tempPigSpawned = PigSpawned;
		ResetFrameFreeze();
		return _tempPigSpawned;
	}

	public override void ResetFrameFreeze()
	{
		PigSpawned = false;
	}
	public bool SetPigSpawnedTrue() => PigSpawned = true;
}
