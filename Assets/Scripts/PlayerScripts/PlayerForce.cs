using System.Collections;
using UnityEngine;
using Zenject;

public class PlayerForce : MonoBehaviour
{

	[SerializeField]
	private Transform _cameraHolder;
	[SerializeField]
	private PlayerStamina _pStamina;
	
	private PlayerInteractions _interaction;
	private Transform _target;
	private float _time = 10;
	private OnLookEvents _onLookEvent;

	[Inject]
	private GameManager _gameManager;

	private void Awake()
	{
		_interaction = GetComponent<PlayerInteractions>();
		_cameraHolder = Camera.main.transform.parent;
	}
	public void SetEvents(OnLookEvents onLookEvent)
	{
		_onLookEvent = onLookEvent;
	}
	public void StartForce(Transform tf)
	{
		_pStamina.Force = true;

		_interaction.StopInteractions(true);
		_target = tf;
		_gameManager.Force = true;
		StartCoroutine(Force());
	}
	private IEnumerator Force()
	{

		while ((Vector3.Distance(transform.position, _target.position) > 0.1f ||
				Quaternion.Angle(transform.rotation, _target.rotation) > 0.1f ||
				Quaternion.Angle(_cameraHolder.rotation, _target.rotation) > 0.1f) &&
			   _gameManager.Force)
		{
			Vector3 currentRotation = transform.rotation.eulerAngles;
			Vector3 targetRotation = _target.rotation.eulerAngles;
			float lerpSpeed = _time * Time.deltaTime;

			float newYRotation = Mathf.LerpAngle(currentRotation.y, targetRotation.y, lerpSpeed);
			transform.rotation = Quaternion.Euler(currentRotation.x, newYRotation, currentRotation.z);

			_cameraHolder.rotation = Quaternion.Slerp(_cameraHolder.rotation, _target.rotation, lerpSpeed);

			transform.position = Vector3.Lerp(transform.position, _target.position, lerpSpeed);

			yield return null;
		}
		_onLookEvent?.ForceEvents.Invoke();
		_onLookEvent = null;
	}

	public void StopForce()
	{
		_pStamina.Force = false;
		_pStamina.ChangeStamina(true);
		_interaction.StopInteractions(false);
		_gameManager.Force = false;
	}
}
