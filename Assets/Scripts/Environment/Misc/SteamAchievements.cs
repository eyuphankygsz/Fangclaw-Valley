using Steamworks;
using System.Collections.Generic;
using UnityEngine;

public class SteamAchievements : MonoBehaviour
{

	[SerializeField]
	private bool _stop;
	public void TryEnableAchievement(AchievementCheck ach)
	{
		if(_stop) return;
		if (ach.IsInt)
		{
			int current = PlayerPrefs.GetInt(ach.CurrentInt);
			if (ach.GreaterThanEqual)
			{
				if (current < ach.TargetInt)
					return;
			}
			else if (ach.GreaterThan)
			{
				if (current <= ach.TargetInt)
					return;
			}
			else if (ach.LessThanEqual)
			{
				if (current > ach.TargetInt)
					return;
			}
			else if (ach.LessThan)
			{
				if (current >= ach.TargetInt)
					return;
			}
			else
			{
				if (current != ach.TargetInt)
					return;
			}
		}
		else if (ach.IsFloat)
		{
			float current = PlayerPrefs.GetInt(ach.CurrentFloat);

			if (ach.GreaterThanEqual)
			{
				if (current < ach.TargetFloat)
					return;
			}
			else if (ach.GreaterThan)
			{
				if (current <= ach.TargetFloat)
					return;
			}
			else if (ach.LessThanEqual)
			{
				if (current > ach.TargetFloat)
					return;
			}
			else if (ach.LessThan)
			{
				if (current >= ach.TargetFloat)
					return;
			}
			else
			{
				if (current != ach.TargetFloat)
					return;
			}
		}
		else if (ach.IsString)
			if (PlayerPrefs.GetString(ach.CurrentString) != ach.TargetString)
				return;

		Debug.Log("Acquired");

		if (SteamManager.Initialized)
		{
			SteamUserStats.GetAchievement(ach.AchievementName, out bool gotAchievement);
			if (!gotAchievement)
			{
				SteamUserStats.SetAchievement(ach.AchievementName);
				SteamUserStats.StoreStats();

			}
		}
	}

	[SerializeField]
	private List<AchievementCheck> _achievementChecks;
	//private void Update()
	//{
	//	if(Input.GetKeyDown(KeyCode.L))
	//		foreach (var item in _achievementChecks)
	//		{
	//			if (item.IsInt)
	//			{
	//				if (!string.IsNullOrEmpty(item.CurrentInt))
	//					PlayerPrefs.DeleteKey(item.CurrentInt);
	//			}
	//			else if (item.IsFloat)
	//			{
	//				if (!string.IsNullOrEmpty(item.CurrentFloat))
	//					PlayerPrefs.DeleteKey(item.CurrentFloat);
	//			}
	//			else if (item.IsString)
	//			{
	//				if (!string.IsNullOrEmpty(item.CurrentString))
	//					PlayerPrefs.DeleteKey(item.CurrentString);
	//			}
	//		}
	//}
}
