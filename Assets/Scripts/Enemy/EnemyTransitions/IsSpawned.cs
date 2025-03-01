
using UnityEngine;

public class IsSpawned : AbstractCondition
{
    private bool _spawned;

    public override bool CheckCondition()
    {
        return _spawned;
    }

    public void SetSpawned(bool spawned)
    {
        _spawned = spawned;
    }
    public override void ResetFrameFreeze()
    {

    }
}
