using UnityEngine;
using Zenject;

public class ZenjectMainMenuInstaller : MonoInstaller
{

	public override void InstallBindings()
	{
		Container.BindInterfacesAndSelfTo<InputManager>().AsSingle();
	}
}