using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingChanger : MonoBehaviour
{
	[SerializeField]
	private Volume _postProcessingVolume;

	private LensDistortion _lensDistortion;
	private DepthOfField _depthOfField;
	private ChromaticAberration _chromaticAberration;
	private ColorAdjustments _colorAdjustments;

	private float _transitionDuration = 1f;

	private float _initialLensDistortion;
	private float _initialFocusDistance;
	private float _initialChromaticAberration;

	private Color _initialColor = new Color(255f / 255f, 95f / 255f, 95f / 255f);
	private float _intensity = 1.1f, _saturation = -65f;


	private Coroutine _smoothTransition;
	private Coroutine _transitionToTargetFirst, _transitionToTargetSecond;
	private void Awake()
	{
		_postProcessingVolume.profile.TryGet<LensDistortion>(out _lensDistortion);
		_postProcessingVolume.profile.TryGet<DepthOfField>(out _depthOfField);
		_postProcessingVolume.profile.TryGet<ChromaticAberration>(out _chromaticAberration);
		_postProcessingVolume.profile.TryGet<ColorAdjustments>(out _colorAdjustments);


		if (_lensDistortion != null) _initialLensDistortion = _lensDistortion.intensity.value;
		if (_depthOfField != null) _initialFocusDistance = _depthOfField.focusDistance.value;
		if (_chromaticAberration != null) _initialChromaticAberration = _chromaticAberration.intensity.value;
		if (_colorAdjustments != null)
		{
			_colorAdjustments.colorFilter.value = _initialColor * _intensity;
			_colorAdjustments.saturation.value = _saturation;
		}
	}
	private Action _mid, _end;
	public void StartProcessChange(float lensD, float depthOF, float chromaticAb, Color color, Action mid, Action end, float? duration = 1)
	{
		_transitionDuration = duration.Value;
		if (_smoothTransition != null)
		{
			StopCoroutine(_smoothTransition);
			_mid?.Invoke();
			_end?.Invoke();
			_smoothTransition = null;
		}

		_mid = mid;
		_end = end;
		_smoothTransition = StartCoroutine(SmoothTransition(lensD, depthOF, chromaticAb, color));
	}

	IEnumerator SmoothTransition(float lensD, float depthOF, float chromaticAb, Color color)
	{
		if (_transitionToTargetFirst != null)
			StopCoroutine(_transitionToTargetFirst);
		_transitionToTargetFirst = StartCoroutine(TransitionToTarget(true, lensD, depthOF, chromaticAb, color));

		yield return _transitionToTargetFirst;



		if (_transitionToTargetSecond != null)
			StopCoroutine(_transitionToTargetSecond);
		_transitionToTargetSecond = StartCoroutine(TransitionToTarget(false, lensD, depthOF, chromaticAb, color));

		yield return _transitionToTargetSecond;
	}

	IEnumerator TransitionToTarget(bool goingToTarget, float lensD, float depthOF, float chromaticAb, Color color)
	{
		if (!goingToTarget)
			_mid?.Invoke();
		float timeElapsed = 0f;
		float tempDuration;
		tempDuration = goingToTarget ? _transitionDuration / 2 : _transitionDuration;

		float startLensDistortion = goingToTarget ? _initialLensDistortion : lensD;
		float endLensDistortion = goingToTarget ? lensD : _initialLensDistortion;

		float startFocusDistance = goingToTarget ? _initialFocusDistance : depthOF;
		float endFocusDistance = goingToTarget ? depthOF : _initialFocusDistance;

		float startChromaticAberration = goingToTarget ? _initialChromaticAberration : chromaticAb;
		float endChromaticAberration = goingToTarget ? chromaticAb : _initialChromaticAberration;

		if (_colorAdjustments != null)
		{
			_colorAdjustments.colorFilter.value = (goingToTarget ? _initialColor : color) * _intensity;
			_colorAdjustments.saturation.value = goingToTarget ? _saturation : 0f;
		}

		while (timeElapsed < tempDuration)
		{
			float t = timeElapsed / tempDuration;

			if (_lensDistortion != null)
				_lensDistortion.intensity.value = Mathf.Lerp(startLensDistortion, endLensDistortion, t);

			if (_depthOfField != null)
				_depthOfField.focusDistance.value = Mathf.Lerp(startFocusDistance, endFocusDistance, t);

			if (_chromaticAberration != null)
				_chromaticAberration.intensity.value = Mathf.Lerp(startChromaticAberration, endChromaticAberration, t);


			if (_colorAdjustments != null)
			{
				Color lerpedColor = Color.Lerp(goingToTarget ? _initialColor : color,
											   goingToTarget ? color : _initialColor, t);
				_colorAdjustments.colorFilter.value = lerpedColor * _intensity;
				_colorAdjustments.saturation.value = Mathf.Lerp(goingToTarget ? _saturation : 0f,
															   goingToTarget ? 0f : _saturation, t);
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
			_colorAdjustments.colorFilter.value = (goingToTarget ? color : _initialColor) * _intensity;
			_colorAdjustments.saturation.value = goingToTarget ? 0f : _saturation; // Set saturation back to initial
		}
		if (!goingToTarget)
		{
			yield return new WaitForSeconds(3f);
			_end?.Invoke();
		}
	}
}
