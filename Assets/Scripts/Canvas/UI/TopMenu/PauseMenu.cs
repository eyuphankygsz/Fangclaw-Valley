using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public static PauseMenu Instance;
	[SerializeField]
	private Color _unSelected, _selected;

	[SerializeField]
	private SectionDict _sectionDict;

	[field: SerializeField]
	public GameObject LoadingScreen { get; private set; }

	private Dictionary<string, TopSections> _sections;
	private TopSections _oldSection;

	private GameObject _content;

	

	private void Awake()
	{
		Instance = this;

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
		SelectSection(_sections[menuName], true);
	}

	private void SelectSection(TopSections section, bool byButtons)
	{
		foreach (var item in _sections)
			UnSelectTitle(item.Value);

		if (!byButtons && _content.activeSelf && section.SectionName == "Main Menu")
		{
			_content.SetActive(false);
			_oldSection = null;
			SetGameSettings(move: true);
			SetMouse(visible: false);
			return;
		}

		SelectTitle(section);
		SetupSection(section);

	}
	private void SetupSection(TopSections section)
	{
		bool same = section == _oldSection;

		if (_oldSection != null)
			_oldSection.Section.SetActive(same);
		_oldSection = section;

		section.Section.SetActive(!same);

		SetMouse(visible: !same);
		_content.SetActive(!same);

		if (same)
			_oldSection = null;

		SetGameSettings(same);
	}
	private void SetGameSettings(bool move)
	{
		PlayerController.Instance.StopMove = !move;
		Time.timeScale = move == true ? 1 : 0;
	}

	private void UnSelectTitle(TopSections section)
	{
		section.Section.SetActive(false);
		section.TitleObject.color = _unSelected;
		section.TitleObject.fontStyle = FontStyles.Normal;
	}
	private void SelectTitle(TopSections section)
	{
		section.Section.SetActive(true);
		section.TitleObject.color = _selected;
		section.TitleObject.fontStyle = FontStyles.Bold;
	}

	private void SetMouse(bool visible)
	{
		Cursor.lockState = visible ? CursorLockMode.None : CursorLockMode.Locked;
		Cursor.visible = visible;
	}
	private void SetControls(ControlSchema controls)
	{
		controls.UI.Inventory.performed += ctx => SelectSection(_sections["Inventory"], false);
		controls.UI.PauseMenu.performed += ctx => SelectSection(_sections["Main Menu"], false);
		controls.UI.Settings.performed += ctx => SelectSection(_sections["Settings"], false);
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