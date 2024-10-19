using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadSaveSettings : MonoBehaviour
{
    [SerializeField]
    private Setting[] _settings;

	public void Start()
	{
        for (int i = 0; i < _settings.Length; i++)
            _settings[i].Load();
    }
}
