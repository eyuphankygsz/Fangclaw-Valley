using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizeEscape : AbstractCondition
{
	private bool Randomize;
	public override bool CheckCondition()
	{
		return Randomize;
	}
	public void ChangeRandomize(bool rand)
	{
		Randomize = rand;
	}
	public override void ResetFrameFreeze() { }
}
