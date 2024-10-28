
using UnityEngine;

public class CantSeePlayer : AbstractCondition
{
    [SerializeField]
    private Transform _viewPoint;
    [SerializeField]
    private float _viewDistance = 15f; // 15 metre ileriye kadar gör
    [SerializeField]
    private float _fovAngle = 45f; // 45 derece FOV
    [SerializeField]
    private int _rayCount = 20;
    [SerializeField]
    private LayerMask _layer;


    [SerializeField]
    private TimeForExitFollow _exitFollow;
    public override bool CheckCondition()
    {
        Vector3 forward = _viewPoint.forward;

        float startAngle = -_fovAngle / 2;

        for (int i = 0; i <= _rayCount; i++)
        {
            float angle = startAngle + (_fovAngle * (i / (float)_rayCount));
            Vector3 direction = Quaternion.Euler(0, angle, 0) * forward;

            Ray ray = new Ray(_viewPoint.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _viewDistance))
            {
                if (hit.collider.gameObject.layer == 6)
                {
                    Debug.DrawLine(_viewPoint.position, hit.point, Color.red);
                    _exitFollow.ResetTime();
                    return false;
                }
            }
            else
            {
                Debug.DrawLine(_viewPoint.position, _viewPoint.position + direction * _viewDistance, Color.green);
            }
        }
        return true;
    }

    private void OnDrawGizmos()
    {
        Vector3 forward = _viewPoint.forward;

        float startAngle = -_fovAngle / 2;
        for (int i = 0; i <= _rayCount; i++)
        {
            float angle = startAngle + (_fovAngle * (i / (float)_rayCount));
            Vector3 direction = Quaternion.Euler(0, angle, 0) * forward;

            Ray ray = new Ray(_viewPoint.position, direction);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, _viewDistance))
            {
                if (hit.collider.gameObject.layer == 6)
                    Gizmos.color = Color.red;
                else
                    Gizmos.color = Color.green;

                Gizmos.DrawLine(_viewPoint.position, _viewPoint.position + (direction * _viewDistance));
            }
            else
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(_viewPoint.position, _viewPoint.position + (direction * _viewDistance));

            }
        }
    }
}
