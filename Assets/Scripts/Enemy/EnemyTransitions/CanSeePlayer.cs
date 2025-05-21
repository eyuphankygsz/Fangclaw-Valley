using UnityEngine;

public class CanSeePlayer : AbstractCondition
{
    [SerializeField]
    private float _viewDistance = 15f;
    [SerializeField]
    private float _fovAngle = 45f;
    [SerializeField]
    private int _rayCount = 20;
    [SerializeField]
    private LayerMask _obstacleLayer; // Layer for obstacles that block vision
    [SerializeField]
    private LayerMask _targetLayer; // Layer for objects we want to detect

    [SerializeField]
    private Transform[] _viewPoints;
    private Vector3 _currentPos;
    private Vector3 _velocity = Vector3.zero;
    private int _viewIndex;

    [SerializeField]
    private TimeForExitFollow _exitFollow;
    [SerializeField]
    private TimeForLostPlayer _timeForLostPlayer;

    private bool _canSee;
    private const float SMOOTH_TIME = 0.1f;
    private const float POSITION_THRESHOLD = 0.1f;

    private void Awake()
    {
        if (_viewPoints == null || _viewPoints.Length == 0)
        {
            Debug.LogError("No view points assigned to CanSeePlayer!");
            enabled = false;
            return;
        }
        _currentPos = _viewPoints[0].position;
    }

    public override bool CheckCondition()
    {
        if(DebugCondition)
            Debug.Log($"CanSeePlayer: {_canSee} at position {_currentPos}");
        return _canSee;
    }

    public void SendRays()
    {
        CheckNextPoint();
        Vector3 forward = _viewPoints[_viewIndex].forward;
        float startAngle = -_fovAngle / 2;
        bool sawTargetThisFrame = false;

        for (int i = 0; i <= _rayCount; i++)
        {
            float angle = startAngle + (_fovAngle * (i / (float)_rayCount));
            Vector3 direction = Quaternion.Euler(0, angle, 0) * forward;
            Ray ray = new Ray(_currentPos, direction);

            // First check for obstacles
            if (Physics.Raycast(ray, out RaycastHit obstacleHit, _viewDistance, _obstacleLayer))
            {
                // If we hit an obstacle, check if there's a target in front of it
                if (Physics.Raycast(ray, out RaycastHit targetHit, obstacleHit.distance, _targetLayer))
                {
                    sawTargetThisFrame = true;
                    Debug.DrawLine(_currentPos, targetHit.point, Color.green, Time.deltaTime);
                }
                else
                {
                    Debug.DrawLine(_currentPos, obstacleHit.point, Color.red, Time.deltaTime);
                }
            }
            else
            {
                // No obstacles, check for targets in full view distance
                if (Physics.Raycast(ray, out RaycastHit targetHit, _viewDistance, _targetLayer))
                {
                    sawTargetThisFrame = true;
                    Debug.DrawLine(_currentPos, targetHit.point, Color.green, Time.deltaTime);
                }
                else
                {
                    Debug.DrawLine(_currentPos, _currentPos + direction * _viewDistance, Color.yellow, Time.deltaTime);
                }
            }
        }

        // Update state and timers
        if (sawTargetThisFrame)
        {
            _canSee = true;
            _exitFollow?.ResetTime();
            _timeForLostPlayer?.ResetTime();
        }
        else
        {
            _canSee = false;
        }
    }

    private void CheckNextPoint()
    {
        if (_viewPoints == null || _viewPoints.Length == 0) return;

        int nextIndex = (_viewIndex + 1) % _viewPoints.Length;
        Vector3 target = _viewPoints[_viewIndex].position;
        
        // Maintain current height while moving to new position
        _currentPos = new Vector3(target.x, _currentPos.y, target.z);
        _currentPos = Vector3.SmoothDamp(_currentPos, _viewPoints[nextIndex].position, ref _velocity, SMOOTH_TIME);

        // Check if we've reached the next point
        bool reachedNextPoint = (_viewIndex < nextIndex && _currentPos.y >= _viewPoints[nextIndex].position.y - POSITION_THRESHOLD) ||
                              (_viewIndex > nextIndex && _currentPos.y <= _viewPoints[nextIndex].position.y + POSITION_THRESHOLD);

        if (reachedNextPoint)
        {
            SetNextPoint();
        }
    }

    private void SetNextPoint()
    {
        int nextIndex = (_viewIndex + 1) % _viewPoints.Length;
        _viewIndex = nextIndex == 0 ? 0 : nextIndex;
        _currentPos = _viewPoints[_viewIndex].position;
    }

    public override void ResetFrameFreeze()
    {
        // Reset any frame freeze related state if needed
    }

    private void OnDrawGizmos()
    {
        if (!Application.isPlaying || _viewPoints == null) return;
        
        // Draw view points
        Gizmos.color = Color.blue;
        foreach (var point in _viewPoints)
        {
            if (point != null)
                Gizmos.DrawSphere(point.position, 0.2f);
        }

        // Draw current view position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(_currentPos, 0.3f);
    }
}
