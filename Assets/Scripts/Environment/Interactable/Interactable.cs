using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public abstract class Interactable : MonoBehaviour, ISaveable
{
	[field: SerializeField]
	public string ObjectName { get; protected set; }

	[field: SerializeField]
	public string InteractableName { get; protected set; }
	public bool IsActive;

	protected bool _used;

	[SerializeField] protected Enum_Weapons[] _includedWeapons;


	[SerializeField]
	protected UnityEvent _oneTimeEvents;
	[SerializeField]
	protected UnityEvent _doneEvents;
	[SerializeField]
	protected UnityEvent _trueEvents;
	[SerializeField]
	protected UnityEvent _falseEvents;
	[SerializeField]
	protected UnityEvent _interactEvents;


	[SerializeField]
	protected GameObject _scanObject;

	[Inject]
	protected SaveManager _saveManager;
	public virtual void OnInteract(Enum_Weapons weapon)
	{
		_saveManager.AddSaveableObject(gameObject, GetSaveFile());

	}
	public virtual void SetStatusManually(bool on)
	{

	}
	public void ChangeObjectName()
	{

	}
	public abstract GameData GetGameData();
	public abstract void LoadData();
	protected void Awake()
	{
		var x = GetSaveFile();
		if (x == null)
			return;

		x.Name = InteractableName;

		if(_scanObject != null)
		{
			Transform mapTransform = _scanObject.transform.GetChild(0);
			mapTransform.parent = null;

			Transform scanParent = _scanObject.transform.parent;
			_scanObject.transform.parent = null;
			_scanObject.transform.localScale = Vector3.one * 0.05f;
			_scanObject.transform.parent = scanParent;

			mapTransform.position = new Vector3(mapTransform.position.x, 52, mapTransform.position.z);
			mapTransform.localScale = Vector3.one * 1;
			mapTransform.parent = _scanObject.transform;
		}
	}
	protected void Start()
	{
		LoadData();
	}

	protected bool IsWeaponInclude(Enum_Weapons e)
	{
		for (int i = 0; i < _includedWeapons.Length; i++)
			if (_includedWeapons[i] == e)
				return true;
		return false;
	}


	protected void OneTimeEvent()
	{
		if (!_used)
		{
			_used = true;
			_oneTimeEvents.Invoke();
		}

	}
	protected void DoneEvent()
	{
		_doneEvents.Invoke();
	}

	private WaitForSeconds _scanTime = new WaitForSeconds(10);
	private Coroutine _scanRoutine;
	public void ShowScanObject()
	{
		if (_scanRoutine != null)
			StopCoroutine(_scanRoutine);

		_scanRoutine = StartCoroutine(ScanRoutine());
	}
	private void OnDisable()
	{
		if (_scanObject != null)
			_scanObject.SetActive(false);
	}

	private IEnumerator ScanRoutine()
	{
		if (_scanObject == null)
			yield break;

		_scanObject?.SetActive(true);
		yield return _scanTime;
		_scanObject?.SetActive(false);

	}


	public GameData GetSaveFile() => GetGameData();

	public void SetLoadFile()
	{
		return;
	}
}