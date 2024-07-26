using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	[SerializeField]
	private Color _unSelected, _selected;

	[SerializeField]
	private SectionDict _sectionDict;

	private Dictionary<string, TopSections> _sections;
	private TopSections _oldSection;

	private GameObject _content;

	private void Awake()
	{
		_content = transform.GetChild(0).gameObject;
		_content.SetActive(false);
	}
	private void Start()
	{
		SetControls(InputManager.Instance.Controls);
		_sections = _sectionDict.ToDict();
	}

	public void ChangeMenu(string menuName)
	{
		DropOldSection();
		SelectSection(_sections[menuName]);
	}

	private void SelectSection(TopSections section)
	{
		Setup(section);

		foreach (var item in _sections)
		{
			item.Value.Section.SetActive(false);
			SetOldSection(item.Value, drop: true);
		}

		section.Section.SetActive(true);
		SetOldSection(section, drop: false);
	}
	private void Setup(TopSections selectedSection)
	{
		if (selectedSection == _oldSection || _oldSection == null)
		{
			SetMouse();
			_content.SetActive(!_content.activeSelf);
			PlayerController.Instance.StopMove = _content.activeSelf;
			Time.timeScale = Time.timeScale == 1 ? 0 : 1;
		}

	}
	private void DropOldSection()
	{
		if (_oldSection == null) return;
		SetOldSection(null, drop: true);
	}

	private void SetOldSection(TopSections section, bool drop)
	{
		if (section != null) _oldSection = section;


		if (drop)
		{
			_oldSection.TitleObject.color = _unSelected;
			_oldSection.TitleObject.fontStyle = FontStyles.Normal;
		}
		else
		{
			_oldSection.TitleObject.color = _selected;
			_oldSection.TitleObject.fontStyle = FontStyles.Bold;
		}
	}

	private void SetMouse()
	{
		if (Cursor.lockState == CursorLockMode.Locked)
		{
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
	private void SetControls(ControlSchema controls)
	{
		controls.UI.Inventory.performed += ctx => SelectSection(_sections["Inventory"]);
		controls.UI.PauseMenu.performed += ctx => SelectSection(_sections["Main Menu"]);
		controls.UI.Settings.performed += ctx => SelectSection(_sections["Settings"]);
	}

}

[Serializable]
public class TopSections
{
	public string SectionName;
	public TextMeshProUGUI TitleObject;
	public GameObject Section;
}
[Serializable]
public class SectionDict
{
	public List<TopSections> _sections;
	public Dictionary<string, TopSections> ToDict()
	{
		Dictionary<string, TopSections> newdict = new Dictionary<string, TopSections>();
		foreach (var item in _sections)
			newdict.Add(item.SectionName, item);
		return newdict;
	}
}