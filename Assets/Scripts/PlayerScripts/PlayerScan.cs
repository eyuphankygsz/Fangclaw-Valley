using UnityEngine;
using UnityEngine.InputSystem;
using System;
using Zenject;

public class PlayerScan : MonoBehaviour, IInputHandler
{
	[SerializeField]
	private float _maxScanRadius;
	[SerializeField]
	private AudioSource _source;
	[SerializeField]
	private AudioClip[] _clips;
	[SerializeField] 
	private LayerMask _layerMask;

	[SerializeField]
	private PlayerStamina _playerStamina;

	private bool _scanning;
	private ControlSchema _controls;

	private float _targetLensDistortion = -0.5f;
	private float _targetFocusDistance = 0f;
	private float _targetChromaticAberration = 1.1f;
	private Color _targetColor = new Color(207f / 255f, 255f / 255f, 0f);


	private bool _freeze;
	public void SetFreeze(bool freeze) => _freeze = freeze;

	[SerializeField]
	private PostProcessingChanger _ppc;

	[SerializeField]
	[Inject]
	private QuestManager _qManager;

	void Awake()
	{
		_midAction += CheckObjects;
		_endAction += EndScan;
	}

	private Action _midAction, _endAction;
	public void Scan(InputAction.CallbackContext ctx)
	{
		if (ctx.performed && !_scanning && !_freeze)
		{
			if (_playerStamina.Stamina < 15)
				return;

			for (int i = 0; i < _clips.Length; i++)
				_source.PlayOneShot(_clips[i]);

			_playerStamina.AddStamina(-15);
			_qManager.ShowQuests();
			_scanning = true;
			_ppc.StartProcessChange(_targetLensDistortion, _targetFocusDistance, _targetChromaticAberration, _targetColor, _midAction, _endAction);
		}
	}

	void CheckObjects()
	{
		Collider[] colliders = Physics.OverlapSphere(transform.position, _maxScanRadius, _layerMask);
		for (int i = 0; i < colliders.Length; i++)
			colliders[i].GetComponent<Interactable>().ShowScanObject();
	}

	void EndScan()
	{
		_scanning = false;
	}

	public void OnInputEnable(ControlSchema schema)
	{
		_controls = schema;
		_controls.Player.Scan.performed += Scan;
	}

	public void OnInputDisable()
	{
		_controls.Player.Scan.performed -= Scan;
	}
}
