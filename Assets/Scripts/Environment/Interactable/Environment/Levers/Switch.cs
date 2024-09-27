using DG.Tweening;
using UnityEngine;

public class Switch : Interactable
{
	[SerializeField]
	private int _id;
	[SerializeField]
	private bool _isOn;
	private bool _animating;

	[SerializeField]
	private Transform _theLever;
	[SerializeField]
	private StatusMechanism _holder;


	private SwitchData _data;

	[SerializeField]
	private AudioSource _audioSource;
	[SerializeField]
	private AudioClip _on, _off, _turn;

	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		Use();
	}
	public override GameData GetGameData()
	{
		_data = new SwitchData()
		{
			Name = InteractableName,
			IsOn = _isOn
		};
		return _data;
	}

	public override void LoadData()
	{
		_data = _saveManager.GetData<SwitchData>(InteractableName);
		if (_data == null) return;

		if (_data.IsOn)
			Use();
		_saveManager.AddSaveableObject(gameObject, GetGameData());
	}

	public void Use()
	{
		if (_animating) return;
		_animating = true;
		_isOn = !_isOn;
		Play(_turn);
		_theLever.DOLocalRotate(new Vector3(_isOn ? -90 : 90, 0, 0), .5f, RotateMode.WorldAxisAdd).OnComplete(RotateEnd);
	}

	private void Play(AudioClip clip)
	{
		_audioSource.clip = clip;
		_audioSource.Play();
	}

	private void RotateEnd()
	{
		_animating = false;
		Play(_isOn ? _on : _off);
		_holder.SetLever(_id, _isOn, false);
	}
}
