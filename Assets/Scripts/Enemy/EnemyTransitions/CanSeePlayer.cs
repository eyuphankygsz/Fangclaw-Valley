
using UnityEngine;

public class CanSeePlayer : AbstractCondition
{
    [SerializeField]
    private float _viewDistance = 15f; // 15 metre ileriye kadar g�r
    [SerializeField]
    private float _fovAngle = 45f; // 45 derece FOV
    [SerializeField]
    private int _rayCount = 20;
    [SerializeField]
    private LayerMask _layer;
    [SerializeField]
    private LayerMask _canSeeLayer;

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

    private void Awake()
    {
        _currentPos = _viewPoints[0].position;
    }
    public override bool CheckCondition()
    {
        return _canSee;
    }

    public void SendRays()
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

            if (Physics.Raycast(ray, out hit, _viewDistance, _layer))
            {
                RaycastHit hitOthers;
                if (Physics.Raycast(ray, out hitOthers, _viewDistance))
                {
					int layer = hitOthers.collider.gameObject.layer;
                    if (layer != 6)
                    {
                        if ((_canSeeLayer.value & (1 << layer)) != 0)
                        {
                            Debug.DrawLine(_currentPos, hit.point, Color.red);
                            _exitFollow.ResetTime();
                            _timeForLostPlayer.ResetTime();
                            _canSee = true;
                        }
                        else
                        {
                            _canSee = false;
						}

					}
                    else
                    {
                        Debug.DrawLine(_currentPos, hit.point, Color.red);
                        _exitFollow.ResetTime();
                        _timeForLostPlayer.ResetTime();
                        _canSee = true;
					}
				}
            }
            else
            {
                Debug.DrawLine(_currentPos, _currentPos + direction * _viewDistance, Color.green);
            }
        }
    }

    private void CheckNextPoint()
    {
        int nextIndex = (_viewIndex + 1) % _viewPoints.Length;
        Vector3 target = _viewPoints[_viewIndex].position;
        _currentPos = new Vector3(target.x, _currentPos.y, target.z);
        _currentPos = Vector3.SmoothDamp(_currentPos, _viewPoints[nextIndex].position, ref _velocity, .1f);

        if ((_viewIndex < nextIndex && _currentPos.y >= _viewPoints[nextIndex].position.y - .1f) || (_viewIndex > nextIndex && _currentPos.y <= _viewPoints[nextIndex].position.y + .1f))
            SetNextPoint();
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

   
    public override void ResetFrameFreeze()
    {

    }
}
