using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerParanoia : MonoBehaviour
{
	[SerializeField]
	private float _normalTime = 10, _currentTime, _currentResetTime, _normalResetTime = 16;
	private WaitForSeconds _attackTime = new WaitForSeconds(1);
	private Coroutine _ariseRoutine, _beatRoutine;

	[SerializeField]
	private PlayerHealth _playerHealth;
	[SerializeField]
	private Volume _vol;



	private float _initialLensDistortion = 0;
	private float _initialBloom = 0;
	private float _initialVignette = 0;

	private float _targetLensDistortion = 0.4f;
	private float _targetBloom = 1.4f;
	private float _targetVignette = 1;

	private bool _isPeak, _beatStarted;

	private LensDistortion _lensDistortion;
	private Bloom _bloom;
	private Vignette _vignette;

	private AudioSource _whisperSfx, _heartBeatSFX;



	private void Awake()
	{
		_whisperSfx = transform.GetChild(0).GetComponent<AudioSource>();
		_heartBeatSFX = transform.GetChild(1).GetComponent<AudioSource>();

		_vol.profile.TryGet<LensDistortion>(out _lensDistortion);
		_vol.profile.TryGet<Bloom>(out _bloom);
		_vol.profile.TryGet<Vignette>(out _vignette);

	}

	void Start()
	{
		_currentTime = _normalTime;
		_currentResetTime = _normalResetTime;
	}

	public void StartParanoiaTimer()
	{
		_whisperSfx.volume = 0;
		_whisperSfx.Play();
		if (_ariseRoutine != null)
			StopCoroutine(_ariseRoutine);

		Debug.Log("START PARANOIA");
		_ariseRoutine = StartCoroutine(ParanoiaArise());
	}
	public void StopParanoiaTimer()
	{
		if (_ariseRoutine != null)
			StopCoroutine(_ariseRoutine);
		_ariseRoutine = StartCoroutine(ParanoiaFade());
	}

	private IEnumerator ParanoiaArise()
	{
		_currentResetTime = _normalResetTime;
		while (true)
		{
			_beating = true;
			if (!_beatStarted)
				StartCoroutine(HeartBeat());


			if (_currentTime <= 0)
			{
				_currentTime = 0;
				_playerHealth.AddHealth(-10);
				yield return _attackTime;
			}
			else
			{
				_currentTime -= Time.deltaTime;
				yield return null;
			}
			AriseProcessing();
		}
	}
	private IEnumerator ParanoiaFade()
	{
		_currentResetTime = Mathf.Lerp(0, _normalResetTime, 1 - (_currentTime / _normalTime));
		while (_currentResetTime > 0)
		{
			FadeProcessing();
			_currentResetTime -= Time.deltaTime;

			float progress = 1 - (_currentResetTime / _normalResetTime);
			_currentTime = Mathf.Lerp(0, _normalTime, progress);

			yield return null;
		}
		_beating = false;
		_whisperSfx.Stop();
		_currentResetTime = 0;
	}

	private void SetStrength(float currentValue, float minValue, float maxValue)
	{
		float strength = Mathf.Abs(_normalTime - _currentResetTime);
		strength = Mathf.InverseLerp(0, _normalTime, strength);



	}

	bool _waveDown;
	private void AriseProcessing()
	{
		float progress = 1 - (_currentTime / _normalTime);
		_isPeak = progress >= 1;

		_whisperSfx.volume = Mathf.Lerp(0, 1, progress);
		_heartBeatSFX.volume = Mathf.Lerp(0.5f, 1, progress);

		if (!_isPeak)
		{
			_lensDistortion.intensity.value = Mathf.Lerp(0, _targetLensDistortion, progress);
			_bloom.intensity.value = Mathf.Lerp(0, _targetBloom, progress);
			_vignette.intensity.value = Mathf.Lerp(0, _targetVignette, progress);
		}
		else
		{
			if (_waveDown)
			{
				float lensTarget = _targetLensDistortion - 0.1f;
				_lensDistortion.intensity.value -= Time.deltaTime;

				if (_lensDistortion.intensity.value <= lensTarget)
					_waveDown = false;
			}
			else
			{
				float lensTarget = _targetLensDistortion + 0.1f;
				_lensDistortion.intensity.value += Time.deltaTime;

				if (_lensDistortion.intensity.value >= lensTarget)
					_waveDown = true;
			}
		}

	}


	private void FadeProcessing()
	{
		float progress = 1 - (_currentTime / _normalTime);

		_whisperSfx.volume = Mathf.Lerp(0, 1, progress);
		_heartBeatSFX.volume = Mathf.Lerp(0.5f, 1, progress);

		_isPeak = false;

		_lensDistortion.intensity.value = Mathf.Lerp(0, _targetLensDistortion, progress);
		_bloom.intensity.value = Mathf.Lerp(0, _targetBloom, progress);
		_vignette.intensity.value = Mathf.Lerp(0, _targetVignette, progress);
	}

	private bool _beating;
	private IEnumerator HeartBeat()
	{
		_beatStarted = true;
		while (_beating)
		{
			float progress = 1 - (_currentTime / _normalTime);
			float time = 1.6f - progress;
			Debug.Log(time);
			yield return new WaitForSeconds(time);
			_heartBeatSFX.Play();
			yield return null;
		}
		_beatStarted = false;
	}




}
