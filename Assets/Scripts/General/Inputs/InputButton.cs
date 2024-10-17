using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Zenject;

public class InputButton : MonoBehaviour
{
	[SerializeField]
	private List<TheInput> _inputs;

	[SerializeField]
	private TextMeshProUGUI _tmp;
	
	[Inject]
	private InputManager _inputManager;

	private int _currentIndex;
    private void Awake()
    {
        _tmp = GetComponentInChildren<TextMeshProUGUI>();
    }
    public void Start()
	{
		_inputManager.SetButtonText(_inputs[0], _tmp);
	}
	public void ChangeKey()
	{
		_inputManager.ChangeBinding(_inputs[_currentIndex], _tmp);
	}

    public TheInput GetInput()
    {
        return _inputs[_currentIndex];
    }
    public TextMeshProUGUI GetTMP()
    {
        return _tmp;
    }
    public void ChangeIndex(int newIndex)
	{
		transform.parent.gameObject.SetActive(!(_inputs.Count <= newIndex));
		if(!transform.parent.gameObject.activeSelf) return;

		_currentIndex = newIndex;
		_inputManager.SetButtonText(_inputs[_currentIndex], _tmp);
	}
}
