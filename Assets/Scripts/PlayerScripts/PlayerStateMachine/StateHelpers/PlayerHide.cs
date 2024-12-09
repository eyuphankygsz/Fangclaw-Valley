using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHide : MonoBehaviour
{
	[SerializeField]
	private PlayerController _controller;
	[SerializeField]
	private Vector3 _hideBoxExtends;
	[SerializeField]
	private LayerMask _hideLayer;
	[SerializeField]
	private Transform _hideBoxTransform;
	[SerializeField]
	private float _castDistance;

	public bool StopHide;
	private Coroutine _stopRoutine;
	public void StopHideStart()
	{
		StopHide = true;
		_controller.Hide(false);
		if (_stopRoutine != null)
			StopCoroutine(_stopRoutine);
		_stopRoutine = StartCoroutine(WaitHide());
	}
	private IEnumerator WaitHide()
	{
		yield return new WaitForSeconds(0.5f);
		StopHide = false;
	}
	public void CheckHide()
	{
		_controller.Hide(StopHide ? false : Physics.BoxCast(_hideBoxTransform.position, _hideBoxExtends, Vector3.up, Quaternion.identity, _castDistance, _hideLayer));
	}

	private void OnDrawGizmos()
	{
		if (_hideBoxTransform == null) return;

		Gizmos.color = Color.red;

		Vector3 endPosition = _hideBoxTransform.position + Vector3.up * _castDistance;

		Gizmos.DrawWireCube(_hideBoxTransform.position, _hideBoxExtends * 2);
		Gizmos.DrawLine(_hideBoxTransform.position, endPosition);
	}
}
