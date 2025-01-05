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
	[SerializeField]
	private PlayerParanoia _playerParanoia;

	public bool StopHide;
	private Coroutine _stopRoutine;


	private bool _hiding;

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
		bool hide = StopHide ? false : Physics.BoxCast(_hideBoxTransform.position, _hideBoxExtends, Vector3.up, Quaternion.identity, _castDistance, _hideLayer);
		if (hide == _hiding) return;
		
		_hiding = hide;

		if (hide)
			_playerParanoia.StartParanoiaTimer();
		else
			_playerParanoia.StopParanoiaTimer();

		_controller.Hide(hide);
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
