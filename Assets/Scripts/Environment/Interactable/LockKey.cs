using System;
using UnityEngine;
using UnityEngine.Localization;

[Serializable]
public class LockKey
{
	[SerializeField]
	private bool _locked;
	[SerializeField]
	private LocalizedString _keyName;

	public bool Locked { get { return _locked; } set { _locked = value; } }
	public string KeyName { get { return _keyName.GetLocalizedString(); } }
}
