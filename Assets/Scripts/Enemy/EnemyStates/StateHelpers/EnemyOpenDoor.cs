using UnityEngine;

public class EnemyOpenDoor : MonoBehaviour
{
	[SerializeField]
	private Transform _checkPoint;
	[SerializeField]
	private float _distance;
	[SerializeField]
	private LayerMask _layerMask;

	[SerializeField]
	private TimeForExitStuck _stuckTimer;

	public void CheckDoors()
	{
		Vector3 forward = _checkPoint.forward;
		Ray ray = new Ray(_checkPoint.position, forward);

		Collider[] colliders = Physics.OverlapSphere(transform.position, 2, _layerMask);
		if (colliders.Length > 0)
		{
			Interactable_HingedObjects hinged;
			if (colliders[0].GetComponent<Collider>().TryGetComponent<Interactable_HingedObjects>(out hinged))
				if (hinged.GetStatus() == false && !hinged.IsLocked())
				{
					hinged.SetStatusManually(true);
					_stuckTimer.ResetTime();
				}
		}
		//if (Physics.Raycast(ray, out hit, _distance, _layerMask))
		//{

		//}
	}
}
