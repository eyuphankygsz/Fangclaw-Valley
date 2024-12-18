using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CombinationPart : MonoBehaviour
{
	private bool _isTurning;
	private Interactable_CombinationManager _combinationManager;

	[SerializeField]
	private int _id, _number, _startNumber;
	[SerializeField]
	private int _maxNumber;

	public bool Loaded;
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
		if (_isTurning) return;

		_isTurning = true;

		if (setManually) 
			_number = (_number + times) % _maxNumber;

		transform.DOLocalRotate(new Vector3(0, 0, 45 * times), .5f, RotateMode.WorldAxisAdd).OnComplete(setManually ? RotateEndManually : RotateEnd);
	}
	public int GetMaxNumber() => _maxNumber;

	private void RotateEnd()
	{
		_number++;

		if (_number > _maxNumber)
			_number = _startNumber;
		_isTurning = false;
		_combinationManager.ChangeCode(_id, _number);
	}

	private void RotateEndManually()
	{
		_isTurning = false;
	}

}
