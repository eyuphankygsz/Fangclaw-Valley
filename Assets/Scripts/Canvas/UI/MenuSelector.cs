using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelector : MonoBehaviour
{
    [SerializeField]
    private GameObject[] _menuList;
    
    
    public void OpenFirst()
    {
        OpenMenu(0);
    }

    public void OpenMenu(int id)
    {
        for (int i = 0; i < _menuList.Length; i++)
            _menuList[i].SetActive(false);

        _menuList[id].SetActive(true);
    }
}
