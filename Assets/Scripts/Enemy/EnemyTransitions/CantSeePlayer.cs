
using UnityEngine;

public class CantSeePlayer : AbstractCondition
{
    [SerializeField]
    private float _viewDistance = 15f; // 15 metre ileriye kadar gör
    [SerializeField]
    private float _fovAngle = 45f; // 45 derece FOV
    [SerializeField]
    private int _rayCount = 20;
    [SerializeField]
    private LayerMask _layer;

    [SerializeField]
    private Transform[] _viewPoints;
    private Vector3 _currentPos;

    private Vector3 _velocity = Vector3.zero;

    private int _viewIndex;


    [SerializeField]
    private TimeForExitFollow _exitFollow;

	private void Awake()
	{
		_currentPos = _viewPoints[0].position;
	}
	public override bool CheckCondition()
    {
        CheckNextPoint();

		Vector3 forward = _viewPoints[_viewIndex].forward;

        float startAngle = -_fovAngle / 2;

        for (int i = 0; i <= _rayCount; i++)
        {
            float angle = startAngle + (_fovAngle * (i / (float)_rayCount));
            Vector3 direction = Quaternion.Euler(0, angle, 0) * forward;

            Ray ray = new Ray(_currentPos, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _viewDistance))
            {
                if (hit.collider.gameObject.layer == 6)
                {
                    Debug.DrawLine(_currentPos, hit.point, Color.red);
                    _exitFollow.ResetTime();
                    return false;
                }
            }
            else
            {
                Debug.DrawLine(_currentPos, _currentPos + direction * _viewDistance, Color.green);
            }
        }
        return true;
    }

    private void CheckNextPoint()
    {
        int nextIndex = (_viewIndex + 1) % _viewPoints.Length;
        Vector3 target = _viewPoints[_viewIndex].position;
        _currentPos = new Vector3(target.x, _currentPos.y, target.z);
        _currentPos = Vector3.SmoothDamp(_currentPos, _viewPoints[nextIndex].position, ref _velocity, 1f);

		if ((_viewIndex < nextIndex && _currentPos.y >= _viewPoints[nextIndex].position.y - .1f) || (_viewIndex > nextIndex && _currentPos.y <= _viewPoints[nextIndex].position.y + .1f))
        {
            Debug.Log("NEXTPOINT||||| VIEW= " + _viewIndex + "  NEXT= " + nextIndex);
            SetNextPoint();
        }
	}
    private void SetNextPoint()
    {
        int nextIndex = (_viewIndex + 1) % _viewPoints.Length;
        if (nextIndex == 0)
        {
            _viewIndex = 0;
            _currentPos = _viewPoints[0].position;
        }
        else
            _viewIndex = nextIndex;
	}

	private void OnDrawGizmos()
    {
        Vector3 forward = _viewPoints[_viewIndex].forward;

        float startAngle = -_fovAngle / 2;
        for (int i = 0; i <= _rayCount; i++)
        {
            float angle = startAngle + (_fovAngle * (i / (float)_rayCount));
            Vector3 direction = Quaternion.Euler(0, angle, 0) * forward;

            Ray ray = new Ray(_currentPos, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _viewDistance))
            {
                if (hit.collider.gameObject.layer == 6)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;

                Gizmos.DrawLine(_currentPos, _currentPos + (direction * _viewDistance));
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(_currentPos, _currentPos + (direction * _viewDistance));

            }
        }
    }
}
