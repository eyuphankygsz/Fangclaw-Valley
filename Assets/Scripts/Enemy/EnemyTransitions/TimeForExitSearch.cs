using UnityEngine;

public class TimeForExitSearch : AbstractCondition
{
    [SerializeField]
    private float _normalTime;
    private float _currentTime;

    private bool _freeze;
    public void ResetTime()
    {
        _currentTime = _normalTime;
    }
    public override bool CheckCondition()
    {
        if (_currentTime <= 0)
            return true;
        if (!_freeze)
            _currentTime -= Time.deltaTime;
        _freeze = true;

        return false;
    }

    public override void ResetFrameFreeze()
    {
        _freeze = false;
    }
}
