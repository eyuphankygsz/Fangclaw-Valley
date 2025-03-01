using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyController
{
	bool DiscardTime { get; set; }
	bool Stunned { get; set; }

	void Shined();
}
