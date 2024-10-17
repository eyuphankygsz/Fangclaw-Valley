using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuSelector : MonoBehaviour
{
    [SerializeField]
    private EventSystem _eventSystem;
    [SerializeField]
    private GameObject[] _menuList;
    [SerializeField]
    private GameObject[] _firstSelected;


    
    public void OpenFirst()
    {
        OpenMenu(0);
    }

    public void OpenMenu(int id)
    {
        _eventSystem.SetSelectedGameObject(_firstSelected[id]);
        for (int i = 0; i < _menuList.Length; i++)
            _menuList[i].SetActive(false);

        _menuList[id].SetActive(true);
    }
}
