using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CombinationPart : MonoBehaviour
{
	public bool IsTurning;
	private Interactable_CombinationManager _combinationManager;

	[SerializeField]
	private int _id, _number, _startNumber;
	[SerializeField]
	private int _maxNumber;

	public bool Loaded;

	[SerializeField]
	private AudioClip[] _clips;
	private AudioSource _source;

	private void Awake()
	{
		_source = GetComponent<AudioSource>();
	}
	private void Start()
	{
		_combinationManager = GetComponentInParent<Interactable_CombinationManager>();
		if (!Loaded)
		{
			_number = _startNumber;
			_combinationManager.ChangeCode(_id, _startNumber);
		}
	}
	public void StartTurnCombination(int times, bool setManually)
	{
		if (IsTurning) return;

		_source.clip = _clips[0];
		_source.Play();

		IsTurning = true;

		if (setManually) 
			_number = (_number + times) % _maxNumber;

		transform.DOLocalRotate(new Vector3(0, 0, 45 * times), .2f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear).OnComplete(setManually ? RotateEndManually : RotateEnd);
	}
	public int GetMaxNumber() => _maxNumber;

	private void RotateEnd()
	{
		_number++; 
		_source.clip = _clips[Random.Range(1,_clips.Length)];
		_source.Play();
		if (_number > _maxNumber)
			_number = _startNumber;
		IsTurning = false;
		_combinationManager.ChangeCode(_id, _number);
	}

	private void RotateEndManually()
	{
		IsTurning = false;
	}

}
