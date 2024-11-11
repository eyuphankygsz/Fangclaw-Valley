using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class IsTurnedToTarget : AbstractCondition
{
	[SerializeField]
	private Transform _target;
	[SerializeField]
	private Transform _enemy;

	public bool CanTurn;
	float yAngleDifference;
	public override bool CheckCondition()
	{
		if (CanTurn)
		{
			//Vector3 relativePos = _target.position - transform.position;
			//Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
			//_enemy.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
			//Debug.Log(yAngleDifference);
			//   yAngleDifference = Mathf.Abs(Mathf.DeltaAngle(_enemy.rotation.eulerAngles.y, rotation.eulerAngles.y));

			Vector3 relativePos = _target.position - transform.position;
			Quaternion targetRotation = Quaternion.LookRotation(relativePos, Vector3.up);
			Quaternion yRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
			_enemy.rotation = Quaternion.RotateTowards(_enemy.rotation, yRotation, Time.deltaTime * 1000);
			
			yAngleDifference = Mathf.Abs(Mathf.DeltaAngle(_enemy.rotation.eulerAngles.y, yRotation.eulerAngles.y));

			if (yAngleDifference <= 1.0f)
				return true;
		}

		return false;
	}

	public override void ResetFrameFreeze() { }
}
