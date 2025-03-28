using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyController
{
	bool DiscardTime { get; set; }
	bool Stunned { get; set; }
	bool IsOnChase { get; set; }
	EnemyStateMachine StateMachine { get; set; }


	void Shined();
	void StopShined();
	bool IsShined();
	void SetChase(int chase);
	void StartAnimationCheck(string animation);
}
