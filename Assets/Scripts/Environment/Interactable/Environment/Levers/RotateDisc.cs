using DG.Tweening;
using UnityEngine;

public class RotateDisc : Interactable
{
	[SerializeField]
	private int _turnID;
	[SerializeField]
	private int _needID;
	[SerializeField]
	private int _currentID;
	[SerializeField]
	private StatusMechanism _holder;
	[SerializeField]
	private int _maxSelectableID;

	private float _rotationAngle;

	private RotateDiscData _data;
	private bool _animating, _atStart;

	private int _times;

	[SerializeField]
	private AudioClip[] _clips;
	private AudioSource _source;

	private void Awake()
	{
		_rotationAngle = 360 / _maxSelectableID;
		base.Awake();
		_source = GetComponent<AudioSource>();
	}

	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		if (_animating) return;
		_animating = true;
		_source.clip = _clips[Random.Range(0, _clips.Length)];
		_source.Play();
		TurnDisc(1);
	}
	public override GameData GetGameData()
	{
		_data = new RotateDiscData()
		{
			Name = InteractableName,
			SelectedID = _currentID
		};
		return _data;
	}

	public override void LoadData()
	{
		_data = _saveManager.GetData<RotateDiscData>(InteractableName);
		if (_data == null) return;

		_atStart = true;
		int turnTimes = GetTurnTimes(_data.SelectedID);
		TurnDisc(turnTimes);

		_saveManager.AddSaveableObject(gameObject, GetGameData());
	}
	private void TurnDisc(int times)
	{
		_times = times;
		transform.DOLocalRotate(new Vector3(-_rotationAngle * times, 0, 0), .5f, RotateMode.WorldAxisAdd).OnComplete(RotateEnd);
	}
	private int GetTurnTimes(int selectedID)
	{
		int count = 0;
		int temp = _currentID;
		while (temp != selectedID)
		{
			count++;
			temp = (temp + 1) % _maxSelectableID;
		}

		return count;

	}

	private void RotateEnd()
	{
		_animating = false;
		_currentID = (_currentID + _times) % _maxSelectableID;
		_holder.SetLever(_turnID, _currentID == _needID, _atStart);
		_atStart = false;
		_source.Stop();
	}
}
