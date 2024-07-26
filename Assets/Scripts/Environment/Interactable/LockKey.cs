using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class LockKey
{
	[SerializeField]
	private bool _locked;
	[SerializeField]
	private string _keyName;

	public bool Locked { get { return _locked; } }
	public string KeyName { get { return _keyName; } }
}
