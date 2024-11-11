using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyOpenDoor : MonoBehaviour
{
	[SerializeField]
	private Transform _checkPoint;
	[SerializeField]
	private float _distance;
	[SerializeField]
	private LayerMask _layerMask;

	public void CheckDoors()
	{
		Vector3 forward = _checkPoint.forward;

		Ray ray = new Ray(_checkPoint.position, forward);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, _distance, _layerMask))
		{
			Interactable_HingedObjects hinged;
			if (hit.collider.TryGetComponent<Interactable_HingedObjects>(out hinged))
				if (hinged.GetStatus() == false && hinged.IsLocked())
					hinged.SetStatusManually(true);
		}
	}
}
