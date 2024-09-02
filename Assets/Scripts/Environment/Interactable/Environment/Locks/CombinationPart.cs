using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CombinationPart : MonoBehaviour
{
	private bool _isTurning;
	private Interactable_CombinationManager _combinationManager;

	[SerializeField]
	private int _id, _number;
	[SerializeField]
	private int _maxNumber;

	private void Start()
	{
		_combinationManager = GetComponentInParent<Interactable_CombinationManager>();
	}
	public void StartTurnCombination(int times, bool setManually)
	{
		if (_isTurning) return;

		_isTurning = true;

		if (setManually) 
			_number = (_number + times + 1) % _maxNumber;

		transform.DOLocalRotate(new Vector3(0, 0, 45 * times), .5f, RotateMode.WorldAxisAdd).OnComplete(setManually ? RotateEndManually : RotateEnd);
	}
	public int GetMaxNumber() => _maxNumber;

	private void RotateEnd()
	{
		_number++;

		if (_number > _maxNumber)
			_number = 0;
		_isTurning = false;
		_combinationManager.ChangeCode(_id, _number);
	}

	private void RotateEndManually()
	{
		_isTurning = false;
	}

}
