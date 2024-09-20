using System.Collections;
using UnityEngine;
using Zenject;

public class Interactable_Crate : Interactable
{
	[Inject]
	private ObjectPool _objectPool;
	[SerializeField] private GameObject _originalObj, _shatterObj, _explosionObj;
	[SerializeField] private bool _allowRandomItem;
	[SerializeField] private RandomItemDrop _randomItemDrop;
	[SerializeField] private Transform _itemTransform;

	[SerializeField]
	private AudioClip[] _clips;
	private AudioSource _source;

	private void Awake()
	{
		base.Awake();
		_source = transform.parent.GetComponent<AudioSource>();
	}

	private CrateData _data = new CrateData();
	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		if (IsWeaponInclude(weapon))
			Shatter();
	}

	private void Shatter()
	{
		_source.PlayOneShot(_clips[Random.Range(0, _clips.Length)]);
		if (_allowRandomItem)
		{
			GameObject obj = null;
			float luck = Random.Range(0f, 1f);
			foreach (var item in _randomItemDrop.Items)
				if (luck <= item.Chance)
				{
					obj = _objectPool.GetObject(_itemTransform.position, item.Item);
					break;
				}

			if (obj != null)
				obj.GetComponent<Interactable_Pickup>().IsCrateItem = true;

		}
		Destroy(_explosionObj, .5f);
		_originalObj.SetActive(false);
		_shatterObj.SetActive(true);
	}
	public override GameData GetGameData()
	{
		_data = new CrateData
		{
			Name = InteractableName,
			IsShattered = _shatterObj.activeSelf,
		};
		return _data;
	}

	public override void LoadData()
	{
		CrateData data = _saveManager.GetData<CrateData>(InteractableName);
		if (data == null) return;

		_saveManager.AddSaveableObject(gameObject, GetSaveFile());

		_originalObj.SetActive(!data.IsShattered);
		_shatterObj.SetActive(data.IsShattered);
		if (data.IsShattered)
			Destroy(_explosionObj, .5f);

	}
}