using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStep : MonoBehaviour
{
	[SerializeField]
	private AudioSource _source;
	[SerializeField]
	private List<ClipsHolder> _clips;

	[SerializeField]
	private PlayerStepCheck _stepCheck;
	
	private Dictionary<StepEnum, AudioClip[]> _clipDictionary;
	private StepEnum _currentEnum = StepEnum.Empty;
	private float _lastStepTime;
	private float _stepTime;
	private ClipsHolder _currentHolder;


	private void Awake()
	{
		_clipDictionary = new Dictionary<StepEnum, AudioClip[]>();
		foreach (var clip in _clips)
			_clipDictionary.Add(clip.Step, clip.Clips);

		_source.clip = _clipDictionary[StepEnum.Wood][0];
		TryChangeStep(_stepCheck.GetStepEnum());
	}

	public void Setup(float stepTime, float firstOffset, float volume)
	{
		_source.volume = volume;
		_lastStepTime = stepTime;
		_stepTime = stepTime - firstOffset;
	}
	public void TryChangeStep(StepEnum newEnum)
	{
		if (_currentEnum == newEnum) return;

		_currentHolder = _clips.First(c => c.Step == newEnum);
		_currentEnum = newEnum;
	}
	public void Step()
	{
		TryChangeStep(_stepCheck.GetStepEnum());

		if (_stepTime <= 0)
			PlayAudio();

		_stepTime -= Time.deltaTime;
		
	}
	private void PlayAudio()
	{
		_stepTime = _lastStepTime;
		_currentHolder.Tracker = (_currentHolder.Tracker + 1) % _clipDictionary[_currentEnum].Length;
		_source.clip = _clipDictionary[_currentEnum][_currentHolder.Tracker];
		_source.Play();
	}
}

[System.Serializable]
public class ClipsHolder
{
	public StepEnum Step;
	public int Tracker;
	public AudioClip[] Clips;
}

public enum StepEnum
{
	Wood,
	Stone,
	Sand,
	Metal,
	Empty
}