using UnityEngine;

public class PlayerStepCheck : MonoBehaviour
{
	[SerializeField]
	private Transform _footTransform;
	[SerializeField]
	private float _castDistance;
	[SerializeField]
	private LayerMask _layerMask;
	[SerializeField]
	private Vector3 _halfExtents;

	private float _hitDistance;
	private bool _hitting;
	public StepEnum GetStepEnum()
	{
		RaycastHit hit;
		if (Physics.BoxCast(_footTransform.position, _halfExtents, -transform.up, out hit, Quaternion.identity, _castDistance, _layerMask))
		{
			_hitting = true;
			_hitDistance = hit.distance;
			return GetEnum(hit.collider.tag);
		}
		_hitting = false;
		return StepEnum.Empty;
	}

	private StepEnum GetEnum(string tag)
	{
		switch (tag)
		{
			case "Wood":
				return StepEnum.Wood;
			case "Stone":
				return StepEnum.Stone;
			case "Sand":
				return StepEnum.Sand;
			case "Metal":
				return StepEnum.Metal;
			default:
				return StepEnum.Empty;
		}
	}

	private void OnDrawGizmos()
	{
		if (_footTransform == null) return;
		if (_hitting)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(_footTransform.position, -transform.up * _hitDistance);
			Gizmos.DrawWireCube(_footTransform.position + -transform.up * _hitDistance, _halfExtents * 2);
		}
		else
		{
			Gizmos.color = Color.green;
			Gizmos.DrawRay(_footTransform.position, -transform.up * _castDistance);
			Gizmos.DrawWireCube(_footTransform.position + -transform.up * _castDistance, _halfExtents * 2);
		}

	}
}
