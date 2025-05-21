using UnityEngine;
using DG.Tweening;
public class Interactable_MusicItem : Interactable
{
	private MusicMechanism _mechanism;
	[SerializeField]
	private char _character;

	[SerializeField]
	private AudioSource _src;
	
	[SerializeField]
	private Animator _anim;

	

#pragma warning disable CS0108 // Member hides inherited member; missing new keyword
	private void Awake()
#pragma warning restore CS0108 // Member hides inherited member; missing new keyword
	{
		_mechanism = GetComponentInParent<MusicMechanism>();
		
		if(_anim == null)
		    _anim = GetComponentInParent<Animator>();
	}
	public override void OnInteract(Enum_Weapons weapon)
	{
		base.OnInteract(weapon);
		_mechanism.AddNote(_character);
		_src?.Play();
		
		_anim.SetTrigger("Play");
	}
	public override GameData GetGameData()
	{
		return null;
	}

	public override void LoadData()
	{
		return;
	}

}
