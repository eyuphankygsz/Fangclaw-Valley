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
	private int _maxNumber, _startNumber;

	private void Start()
	{
		_combinationManager = GetComponentInParent<Interactable_CombinationManager>();
		_combinationManager.ChangeCode(_id, _startNumber);
		_number = _startNumber;

	}
	public void StartTurnCombination()
	{
		if (_isTurning) return;

		_isTurning = true;

		transform.DOLocalRotate(new Vector3(0, 0, 45), .5f, RotateMode.WorldAxisAdd).OnComplete(RotateEnd);
	}

	private void RotateEnd()
	{
		_number++;

		if (_number > _maxNumber)
			_number = _startNumber;
		_isTurning = false;
		_combinationManager.ChangeCode(_id, _number);
	}

}
