
using UnityEngine;

public class RunStaminaCondition : AbstractCondition
{
	[SerializeField]
	private PlayerStamina _playerStamina;
	[SerializeField]
	private int _minStamina;
	[SerializeField]
	private AudioSource _source;
	[SerializeField]
	private AudioClip _clip;


	private bool _playing;
	public override bool CheckCondition()
	{
		if (_playerStamina.Stamina > _minStamina)
		{
			_playing = false;
			_source.Stop();
			return true;
		}
		if (!_playing)
		{
			_playing = true;
			_source.clip = _clip;
			_source.Play();
		}

		return false;
	}

    public override void ResetFrameFreeze()
    {
 
	}
}
