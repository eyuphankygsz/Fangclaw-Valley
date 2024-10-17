using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class ControlType : MonoBehaviour
{
    [SerializeField]
    private List<InputButton> _buttons;
    [SerializeField]
    private LocalizedString[] _controlNames;
    [SerializeField]
    private TextMeshProUGUI _text;

    private int _selectedIndex = 0;
    
    void Start()
    {
        SetButtons(0);
    }

    public void ChangeController()
    {
        _selectedIndex++;
        _selectedIndex %= _controlNames.Length;
        SetButtons(_selectedIndex);
    }
    private void SetButtons(int index)
    {
        _text.text = _controlNames[index].GetLocalizedString();
        foreach (var button in _buttons)
            button.ChangeIndex(index);
    }
}
