using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using UnityEngine.Rendering.Universal;
using UnityEditor.ShaderGraph;
using UnityEngine.InputSystem;

public class PlayerScan : MonoBehaviour, IInputHandler
{
	[SerializeField]
	private float _maxScanRadius;
	[SerializeField]
	private Volume _postProcessingVolume;
	[SerializeField]
	private AudioSource _source;
	[SerializeField]
	private AudioClip[] _clips;
	[SerializeField] 
	private LayerMask _layerMask;

	[SerializeField]
	private PlayerStamina _playerStamina;

	private bool _scanning;
	private ControlSchema _controls;
	private float _currentRadius;

	private LensDistortion _lensDistortion;
	private DepthOfField _depthOfField;
	private ChromaticAberration _chromaticAberration;
	private ColorAdjustments _colorAdjustments;

	private float _transitionDuration = 1f;

	private float _initialLensDistortion = 0f;
	private float _targetLensDistortion = -0.5f;

	private float _initialFocusDistance = 1f;
	private float _targetFocusDistance = 0f;

	private float _initialChromaticAberration = 0f;
	private float _targetChromaticAberration = 1f;

	private Color _initialColor = new Color(200f / 255f, 56f/255f, 56f / 255f);
	private Color _targetColor = new Color(210f / 255f, 0f, 0f);


	void Awake()
	{

		if (_postProcessingVolume == null)
		{
			Debug.LogError("Post Processing Volume is not assigned.");
			return;
		}

		_postProcessingVolume.profile.TryGet<LensDistortion>(out _lensDistortion);
		_postProcessingVolume.profile.TryGet<DepthOfField>(out _depthOfField);
		_postProcessingVolume.profile.TryGet<ChromaticAberration>(out _chromaticAberration);
		_postProcessingVolume.profile.TryGet<ColorAdjustments>(out _colorAdjustments);

		if (_lensDistortion != null) _lensDistortion.intensity.value = _initialLensDistortion;
		if (_depthOfField != null) _depthOfField.focusDistance.value = _initialFocusDistance;
		if (_chromaticAberration != null) _chromaticAberration.intensity.value = _initialChromaticAberration;
		if (_colorAdjustments != null)
		{
			_colorAdjustments.colorFilter.value = _initialColor;
			_colorAdjustments.saturation.value = -100f; // Start with -100 saturation
		}
	}

	public void Scan(InputAction.CallbackContext ctx)
	{
		if (ctx.performed && !_scanning)
		{
			if (_playerStamina.Stamina < 60)
				return;

			_currentRadius = 0;

			_playerStamina.AddStamina(-60);
			_scanning = true;
			StartCoroutine(SmoothTransition());
		}
	}

	IEnumerator SmoothTransition()
	{
		for (int i = 0; i < _clips.Length; i++)
			_source.PlayOneShot(_clips[i]);

		yield return StartCoroutine(TransitionToTarget(true));

		yield return StartCoroutine(TransitionToTarget(false));
	}

	IEnumerator TransitionToTarget(bool goingToTarget)
	{
		float timeElapsed = 0f;
		_transitionDuration = goingToTarget ? 0.5f : 1f;

		float startLensDistortion = goingToTarget ? _initialLensDistortion : _targetLensDistortion;
		float endLensDistortion = goingToTarget ? _targetLensDistortion : _initialLensDistortion;

		float startFocusDistance = goingToTarget ? _initialFocusDistance : _targetFocusDistance;
		float endFocusDistance = goingToTarget ? _targetFocusDistance : _initialFocusDistance;

		float startChromaticAberration = goingToTarget ? _initialChromaticAberration : _targetChromaticAberration;
		float endChromaticAberration = goingToTarget ? _targetChromaticAberration : _initialChromaticAberration;

		if (_colorAdjustments != null)
		{
			_colorAdjustments.colorFilter.value = goingToTarget ? _initialColor : _targetColor; // Start with initial color
			_colorAdjustments.saturation.value = goingToTarget ? -100f : 0f; // Start with -100 saturation
		}

		while (timeElapsed < _transitionDuration)
		{
			float t = timeElapsed / _transitionDuration;

			if (_lensDistortion != null)
				_lensDistortion.intensity.value = Mathf.Lerp(startLensDistortion, endLensDistortion, t);

			if (_depthOfField != null)
				_depthOfField.focusDistance.value = Mathf.Lerp(startFocusDistance, endFocusDistance, t);

			if (_chromaticAberration != null)
				_chromaticAberration.intensity.value = Mathf.Lerp(startChromaticAberration, endChromaticAberration, t);

			if (goingToTarget)
			{
				_currentRadius = Mathf.Lerp(0, _maxScanRadius, t);

				Collider[] colliders = Physics.OverlapSphere(transform.position, _currentRadius, _layerMask);
				for (int i = 0; i < colliders.Length; i++)
				{
					Debug.Log(colliders[i].gameObject);
					colliders[i].GetComponent<Interactable>().ShowScanObject();
				}
			}
			
			if (_colorAdjustments != null)
			{
				Color lerpedColor = Color.Lerp(goingToTarget ? _initialColor : _targetColor,
											   goingToTarget ? _targetColor : _initialColor, t);
				_colorAdjustments.colorFilter.value = lerpedColor;
				_colorAdjustments.saturation.value = Mathf.Lerp(goingToTarget ? -100f : 0f,
															   goingToTarget ? 0f : -100f, t);
			}

			timeElapsed += Time.deltaTime;
			yield return null;
		}

		if (_lensDistortion != null)
			_lensDistortion.intensity.value = endLensDistortion;

		if (_depthOfField != null)
			_depthOfField.focusDistance.value = endFocusDistance;

		if (_chromaticAberration != null)
			_chromaticAberration.intensity.value = endChromaticAberration;

		if (_colorAdjustments != null)
		{
			_colorAdjustments.colorFilter.value = goingToTarget ? _targetColor : _initialColor;
			_colorAdjustments.saturation.value = goingToTarget ? 0f : -100f; // Set saturation back to initial
		}

		if (!goingToTarget)
			_scanning = false;
		_currentRadius = 0;
	}

	public void OnInputEnable(ControlSchema schema)
	{
		_controls = schema;
		_controls.Player.Scan.performed += Scan;
	}

	public void OnInputDisable()
	{
		_controls.Player.Scan.performed -= Scan;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, _currentRadius);
	}
}
