using UnityEngine;

public class Interactable_Crate : Interactable
{
	[SerializeField] private GameObject _originalObj, _shatterObj;
	[SerializeField] private bool _allowRandomItem;
	[SerializeField] private RandomItemDrop _randomItemDrop;
	[SerializeField] private Transform _itemTransform;

	private bool _itemTaken;
	private bool _empty;
	public override void OnInteract(Enum_Weapons weapon)
	{
		if (IsWeaponInclude(weapon))
			Shatter();
	}

	private void Shatter()
	{
		if (_allowRandomItem)
		{
			GameObject obj = null;
			_empty = true;
			float luck = Random.Range(0f, 1f);
			foreach (var item in _randomItemDrop.Items)
				if (luck <= item.Chance)
				{
					_empty = false;
					obj = ObjectPool.Instance.GetObject(_itemTransform.position, item.Item);
					break;
				}

			if (obj != null) 
				SaveManager.Instance.AddCrateItem(obj.GetComponent<Interactable>());
		}

		_originalObj.SetActive(false);
		_shatterObj.SetActive(true);
	}
	public override InteractableData SaveData()
	{
		return new CrateData
		{
			InteractableName = InteractableName,
			Position = transform.position,
			IsActive = gameObject.activeSelf,
			IsShattered = !_originalObj.activeSelf,
			HasItem = _itemTaken,
			IsEmpty = _empty
		};
	}

	public override void LoadData()
	{
		CrateData data = (CrateData)SaveManager.Instance.GetData(InteractableName, SaveType.Crate);
		if (data == null) return;

		transform.position = data.Position;
		gameObject.SetActive(data.IsActive);
		_originalObj.SetActive(!data.IsShattered);
		_shatterObj.SetActive(data.IsShattered);
		_itemTaken = data.HasItem;
	}
}