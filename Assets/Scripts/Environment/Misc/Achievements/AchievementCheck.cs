using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Achievement", menuName = "Achievements/New Achievement")]
public class AchievementCheck : ScriptableObject
{
	public string AchievementName;

	public bool IsInt;
	public bool IsFloat;
	public bool IsString;

	public int TargetInt;
	public float TargetFloat;
	public string TargetString;

	public string CurrentInt;
	public string CurrentFloat;
	public string CurrentString;



	public bool LessThanEqual, GreaterThanEqual, LessThan, GreaterThan;
}